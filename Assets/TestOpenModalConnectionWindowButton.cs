using UnityEngine;
using UnitonConnect.Core.Demo;

namespace UnitonConnect.Core.Data
{
    public sealed class TestOpenModalConnectionWindowButton : TestBaseButton
    {
        [SerializeField, Space] private TestWalletInterfaceAdapter _interfaceAdapter;

        public sealed override void OnClick()
        {
            Debug.Log("Click on Connect");
            _interfaceAdapter.UnitonSDK.Connect();
        }
    }
}