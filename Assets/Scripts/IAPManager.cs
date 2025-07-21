using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System.Threading.Tasks;
using System.Collections;
using TMPro;
using System;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    public static string HealthPlus1ProductId = "com.likemono.sso.healthplus1";
    public static string HealthPlus2ProductId = "com.likemono.sso.healthplus2";
    public static string NoAdsProductId = "com.likemono.sso.noad";
    public static string RemoveBannerProductId = "com.likemono.sso.removebanner";

    public static IAPManager Instance { get; private set; }

    [SerializeField]
    private Button restorePurchasesButton;
    [SerializeField]
    private Button healthPlus1Button; // Health Plus 1 구매 버튼
    [SerializeField]
    private Button healthPlus2Button; // Health Plus 2 구매 버튼

    [SerializeField]
    private TextMeshProUGUI messageText; // 메시지를 표시할 TextMeshProUGUI 필드

    private Button removeBannerAdsButton;
    private Button removeInterstitialAdsButton;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Unity Gaming Services 초기화
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName("production");

            await UnityServices.InitializeAsync(options);
            Debug.Log("Unity Gaming Services initialized successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to initialize Unity Gaming Services: " + e.Message);
        }

        GameManager gameManager = GameManager.instance;
        removeBannerAdsButton = gameManager.removeBannerAdsButton;
        removeInterstitialAdsButton = gameManager.removeInterstitialAdsButton;
    }

    void Start()
    {
        if (storeController == null)
        {
            InitializePurchasing();
        }

        // 복구 버튼이 눌렸을 때 복구 메서드 실행
        restorePurchasesButton.onClick.AddListener(RestorePurchases);

        // 구매 버튼이 한 번만 등록되었는지 확인
        if (healthPlus1Button != null && healthPlus1Button.onClick.GetPersistentEventCount() == 0)
        {
            healthPlus1Button.onClick.AddListener(BuyHealthPlus1);
        }

        if (healthPlus2Button != null && healthPlus2Button.onClick.GetPersistentEventCount() == 0)
        {
            healthPlus2Button.onClick.AddListener(BuyHealthPlus2);
        }

        // 앱 시작 시 이전 구매를 반영하여 UI 업데이트
        UpdateUIBasedOnPurchases();
    }

    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true); // 메시지 표시
        yield return new WaitForSeconds(duration); // 지정된 시간만큼 대기
        messageText.gameObject.SetActive(false); // 메시지 숨김
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        if (messageText != null)
        {
            StartCoroutine(ShowMessageCoroutine(message, duration));
        }
        else
        {
            Debug.LogWarning("Message Text is not assigned.");
        }
    }

    private bool isInitializing = false;

    public void InitializePurchasing()
    {
        if (IsInitialized() || isInitializing)
        {
            Debug.Log("IAP is already initialized or currently initializing.");
            return;
        }

        isInitializing = true;

        try
        {
#if UNITY_ANDROID
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));
#elif UNITY_IOS
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.AppleAppStore));
#endif

            builder.AddProduct(NoAdsProductId, ProductType.NonConsumable);
            builder.AddProduct(RemoveBannerProductId, ProductType.NonConsumable);
            builder.AddProduct(HealthPlus1ProductId, ProductType.NonConsumable);
            builder.AddProduct(HealthPlus2ProductId, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }
        catch (Exception ex)
        {
            Debug.LogError($"IAP initialization failed: {ex.Message}");
            isInitializing = false;
            StartCoroutine(RetryInitialization());
        }
    }

    private IEnumerator RetryInitialization()
    {
        yield return new WaitForSeconds(2f); // 2초 대기 후 재시도
        InitializePurchasing();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        isInitializing = false;
        Debug.Log("IAP successfully initialized.");
    }

    private bool IsInitialized()
    {
        return storeController != null && extensionProvider != null;
    }

    public void BuyHealthPlus1()
    {
        if (storeController != null && healthPlus1Button.interactable)
        {
            healthPlus1Button.interactable = false; // 버튼을 비활성화하여 중복 클릭 방지
            BuyProductID(HealthPlus1ProductId);
        }
    }

    public void BuyHealthPlus2()
    {
        if (storeController != null && healthPlus2Button.interactable)
        {
            healthPlus2Button.interactable = false; // 버튼을 비활성화하여 중복 클릭 방지
            BuyProductID(HealthPlus2ProductId);
        }
    }

    public void BuyNoAds()
    {
        if (removeInterstitialAdsButton != null)
        {
            removeInterstitialAdsButton.interactable = false;
        }
        BuyProductID(NoAdsProductId);
    }

    public void BuyRemoveBanner()
    {
        if (removeBannerAdsButton != null)
        {
            removeBannerAdsButton.interactable = false;
        }
        BuyProductID(RemoveBannerProductId);
    }

    public void BuyProductID(string productId)
    {
        if (!IsInitialized())
        {
            Debug.LogError("BuyProductID failed: IAP is not initialized.");
            ShowMessage("IAP is not initialized. Please try again later.", 3f);
            return;
        }

        Product product = storeController.products.WithID(productId);

        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"Purchasing product asynchronously: {product.definition.id}");
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogError($"BuyProductID failed: Product not found or unavailable for purchase. Product ID: {productId}");
            ShowMessage("Product not available for purchase.", 3f);
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.LogWarning("IAP Manager is not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            var apple = extensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result, msg) => {
                Debug.Log("Restoration process completed. Result: " + result);

                if (result)
                {
                    GameManager.instance.UpdateUIBasedOnPurchases();
                    GameManager.instance.ShowMessage("Purchases restored successfully.");
                }
                else
                {
                    GameManager.instance.ShowMessage("Failed to restore purchases.");
                }
            });
        }
        else
        {
            Debug.Log("RestorePurchases is not supported on this platform. Current platform: " + Application.platform.ToString());
        }
    }    

    public void OnInitializeFailed(InitializationFailureReason reason)
    {
        Debug.LogError($"IAP Initialization Failed: {reason}");
    }

    public void OnInitializeFailed(InitializationFailureReason reason, string message)
    {
        Debug.LogError($"IAP Initialization Failed: {reason} - {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed: {product.definition.id}, reason: {failureReason}");

        // 실패 원인에 따라 적절한 처리를 추가
        switch (failureReason)
        {
            case PurchaseFailureReason.PaymentDeclined:
                Debug.LogWarning("Payment was declined.");
                break;
            case PurchaseFailureReason.ProductUnavailable:
                Debug.LogWarning("Product is unavailable.");
                break;
            case PurchaseFailureReason.UserCancelled:
                Debug.Log("User cancelled the purchase.");
                break;
            default:
                Debug.LogError("Purchase failed for unknown reasons.");
                break;
        }

        // 실패 후 버튼을 다시 활성화 (필요한 경우)
        if (product.definition.id == HealthPlus1ProductId)
        {
            if (healthPlus1Button != null)
                healthPlus1Button.interactable = true;
        }
        else if (product.definition.id == HealthPlus2ProductId)
        {
            if (healthPlus2Button != null)
                healthPlus2Button.interactable = true;
        }
        else if (product.definition.id == NoAdsProductId)
        {
            if (removeInterstitialAdsButton != null)
                removeInterstitialAdsButton.interactable = true;
        }
        else if (product.definition.id == RemoveBannerProductId)
        {
            if (removeBannerAdsButton != null)
                removeBannerAdsButton.interactable = true;
        }

        ShowMessage("Purchase cancelled or failed.", 2f);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("ProcessPurchase called for: " + args.purchasedProduct.definition.id);
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();  // 인게임 중이라면 즉시 업데이트

        if (args.purchasedProduct.definition.id == HealthPlus1ProductId)
        {
            PlayerPrefs.SetInt("HealthPlus1Purchased", 1);  // 구매 정보 저장
            PlayerPrefs.Save();

            // 게임 중이라면 바로 업데이트
            if (playerHealth != null)
            {
                playerHealth.IncreaseMaxHealth(1);
                Debug.Log("Health +1 purchase completed.");
            }
            else
            {
                Debug.Log("PlayerHealth not found - will update on game start.");
            }

            ShowMessage("Health +1 purchased successfully!", 3f);
            return PurchaseProcessingResult.Complete;
        }
        else if (args.purchasedProduct.definition.id == HealthPlus2ProductId)
        {
            PlayerPrefs.SetInt("HealthPlus2Purchased", 1);  // 구매 정보 저장
            PlayerPrefs.Save();

            // 게임 중이라면 바로 업데이트
            if (playerHealth != null)
            {
                playerHealth.IncreaseMaxHealth(1);
                Debug.Log("Health +2 purchase completed.");
            }
            else
            {
                Debug.Log("PlayerHealth not found - will update on game start.");
            }

            ShowMessage("Health +2 purchased successfully!", 3f);
            return PurchaseProcessingResult.Complete;
        }
        else if (args.purchasedProduct.definition.id == NoAdsProductId)
        {
            Debug.Log("No Ads purchase completed.");
            PlayerPrefs.SetInt("NoAds", 1);
            PlayerPrefs.Save();
            GameManager.instance.UpdateUIBasedOnPurchases();
            ShowMessage("No Ads purchase completed!", 3f);
            return PurchaseProcessingResult.Complete;
        }
        else if (args.purchasedProduct.definition.id == RemoveBannerProductId)
        {
            Debug.Log("Remove Banner purchase completed.");
            PlayerPrefs.SetInt("RemoveBanner", 1);
            PlayerPrefs.Save();
            GameManager.instance.UpdateUIBasedOnPurchases();
            ShowMessage("Remove Banner purchase completed!", 3f);
            return PurchaseProcessingResult.Complete;
        }
        return PurchaseProcessingResult.Pending;
    }

    public void UpdateUIBasedOnPurchases()
    {
        if (PlayerPrefs.GetInt("NoAds", 0) == 1)
        {
            DisableNoAdsButton();
        }

        if (PlayerPrefs.GetInt("RemoveBanner", 0) == 1)
        {
            DisableRemoveBannerButton();
        }

        if (PlayerPrefs.GetInt("HealthPlus1Purchased", 0) == 1)
        {
            DisableHealthPlus1Button();
        }

        if (PlayerPrefs.GetInt("HealthPlus2Purchased", 0) == 1)
        {
            DisableHealthPlus2Button();
        }
    }

    private void DisableNoAdsButton()
    {
        if (removeInterstitialAdsButton != null)
        {
            removeInterstitialAdsButton.gameObject.SetActive(false);
        }
        Debug.Log("No Ads button disabled.");
    }

    private void DisableRemoveBannerButton()
    {
        if (removeBannerAdsButton != null)
        {
            removeBannerAdsButton.gameObject.SetActive(false);
        }
        Debug.Log("Remove Banner button disabled.");
    }

    private void DisableHealthPlus1Button()
    {
        if (healthPlus1Button != null)
        {
            healthPlus1Button.gameObject.SetActive(false);
        }
        Debug.Log("Health Plus1 Button disabled.");
    }

    private void DisableHealthPlus2Button()
    {
        if (healthPlus2Button != null)
        {
            healthPlus2Button.gameObject.SetActive(false);
        }
        Debug.Log("Health Plus2 Button disabled.");
    }
}
