using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnitonConnect.Core.Data;
using UnitonConnect.Core.Common;

namespace UnitonConnect.Core.Demo
{
    public sealed class WalletInterfaceAdapter : MonoBehaviour
    {
        [SerializeField, Space] private TextMeshProUGUI _debugMessage;
        [SerializeField] private TextMeshProUGUI _shortWalletAddress;
        [SerializeField, Space] private Button _connectButton;
        [SerializeField] private Button _disconnectButton;

        private UnitonConnectSDK _unitonSDK;
        private WalletModal _walletModal;

        public UnitonConnectSDK UnitonSDK => _unitonSDK;

        private void Awake()
        {
            _unitonSDK = UnitonConnectSDK.Instance;

            Debug.Log("=== ADAPTER AWAKE ===");
            Debug.Log($"SDK GameObject: {_unitonSDK.gameObject.name}");
            Debug.Log($"SDK Scene: {_unitonSDK.gameObject.scene.name}");
            Debug.Log($"Current Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            Debug.Log($"SDK IsInitialized: {_unitonSDK.IsInitialized}");
            Debug.Log($"SDK IsWalletConnected: {_unitonSDK.IsWalletConnected}");
            Debug.Log($"SDK Wallet: {_unitonSDK.Wallet}");

            // Subscribe events
            _unitonSDK.OnInitiliazed += SdkInitialized;
            _unitonSDK.OnWalletConnected += WalletConnectionFinished;
            _unitonSDK.OnWalletConnectFailed += WalletConnectionFailed;
            _unitonSDK.OnWalletConnectRestored += WalletConnectionRestored;
            _unitonSDK.OnWalletDisconnected += WalletDisconnected;
        }

        private void OnDestroy()
        {
            _unitonSDK.OnInitiliazed -= SdkInitialized;
            _unitonSDK.OnWalletConnected -= WalletConnectionFinished;
            _unitonSDK.OnWalletConnectFailed -= WalletConnectionFailed;
            _unitonSDK.OnWalletConnectRestored -= WalletConnectionRestored;
            _unitonSDK.OnWalletDisconnected -= WalletDisconnected;

            if (_walletModal != null)
            {
                _walletModal.OnStateClaimed -= ModalStateClaimed;
                _walletModal.OnStateChanged -= ModalStateChanged;
            }
        }

        private void Start()
        {
            Debug.Log("=== ADAPTER START ===");
            Debug.Log($"SDK IsInitialized: {_unitonSDK.IsInitialized}");
            Debug.Log($"SDK IsWalletConnected: {_unitonSDK.IsWalletConnected}");
            Debug.Log($"SDK Wallet: {_unitonSDK.Wallet}");

            if (!_unitonSDK.IsInitialized)
            {
                _unitonSDK.Initialize();
            }

            if (_unitonSDK.IsWalletConnected && _unitonSDK.Wallet != null)
            {
                Debug.Log("Wallet already connected, update UI now");
                WalletConnectionFinished(_unitonSDK.ConnectedWalletConfig);
            }
            else
            {
                // not connect, disable disconnect button
                _disconnectButton.interactable = false;
            }
        }

        private void SdkInitialized(bool isSuccess)
        {
            if (!isSuccess)
            {
                Debug.LogError("Failed to initialize sdk");
                return;
            }

            Debug.Log("SDK Initialized successfully");

            _connectButton.interactable = true;
            _walletModal = _unitonSDK.Modal;

            _walletModal.OnStateClaimed += ModalStateClaimed;
            _walletModal.OnStateChanged += ModalStateChanged;

            _walletModal.LoadStatus();
        }

        private void WalletConnectionFinished(WalletConfig wallet)
        {
            Debug.Log("=== WALLET CONNECTION FINISHED ===");
            Debug.Log($"Wallet config: {wallet}");
            Debug.Log($"Wallet address: {wallet.Address}");
            Debug.Log($"SDK IsWalletConnected: {_unitonSDK.IsWalletConnected}");
            Debug.Log($"SDK Wallet object: {_unitonSDK.Wallet}");

            if (_unitonSDK.IsWalletConnected && _unitonSDK.Wallet != null)
            {
                var userAddress = wallet.Address;

                var successConnectMessage = $"Wallet is connected\n" +
                    $"Address: {userAddress}\n" +
                    $"Public Key: {wallet.PublicKey}";

                string shortWalletAddress = "";
                if (!string.IsNullOrEmpty(userAddress) && userAddress.Length > 10)
                {
                    shortWalletAddress = userAddress.Substring(0, 6) + "..." +
                                        userAddress.Substring(userAddress.Length - 4);
                }

                _debugMessage.text = successConnectMessage;
                _shortWalletAddress.text = shortWalletAddress;

                Debug.Log($"Short wallet address: {shortWalletAddress}");

                _connectButton.interactable = false;
                _disconnectButton.interactable = true;
            }
            else
            {
                Debug.LogWarning("Wallet object is null!");
            }
        }

        private void WalletConnectionFailed(string message)
        {
            Debug.LogError($"Failed to connect wallet: {message}");

            _connectButton.interactable = true;
            _disconnectButton.interactable = false;
            _debugMessage.text = string.Empty;
            _shortWalletAddress.text = string.Empty;
        }

        private void WalletConnectionRestored(bool isRestored)
        {
            if (!isRestored)
            {
                return;
            }

            Debug.Log("Wallet connection restored");

            _connectButton.interactable = false;
            _disconnectButton.interactable = true;

            // update ui
            if (_unitonSDK.Wallet != null)
            {
                WalletConnectionFinished(_unitonSDK.ConnectedWalletConfig);
            }
        }

        private void WalletDisconnected(bool isSuccess)
        {
            if (!isSuccess)
            {
                return;
            }

            Debug.Log("Wallet disconnected");

            _connectButton.interactable = true;
            _disconnectButton.interactable = false;
            _debugMessage.text = string.Empty;
            _shortWalletAddress.text = string.Empty;
        }

        private void ModalStateChanged(ModalStatusTypes state)
        {
            Debug.Log($"Modal state changed: '{state}'");
        }

        private void ModalStateClaimed(ModalStatusTypes state)
        {
            Debug.Log($"Modal state claimed: '{state}'");
        }
    }
}