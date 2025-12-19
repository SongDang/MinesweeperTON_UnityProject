using UnityEngine;
using UnitonConnect.Core.Demo;

namespace UnitonConnect.Core.Data
{
    public sealed class OpenModalConnectionWindowButton : TestBaseButton
    {
        [SerializeField, Space] private WalletInterfaceAdapter _interfaceAdapter;

        public sealed override void OnClick()
        {
            Debug.Log("Click on Connect");
            _interfaceAdapter.UnitonSDK.Connect();
        }
    }
}