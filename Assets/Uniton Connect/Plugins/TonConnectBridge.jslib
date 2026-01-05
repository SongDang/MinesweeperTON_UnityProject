
const tonConnectBridge = {

    // Class definition

    $tonConnect: {
        CONTRACT_ADDRESS: "kQBuyOVR854P_CSCzvH6FJnMTLqgRs2ekaBTobhOeMQuYcXx",

        allocString: function (stringData)
        {
            let ptr;

            if (typeof allocate === 'undefined')
            {
                const length = lengthBytesUTF8(stringData) + 1;

                ptr = _malloc(length);

                stringToUTF8(stringData, ptr, length);

                return ptr;
            }

            return allocate(intArrayFromString(stringData), 'i8', ALLOC_NORMAL);
        },

        isAvailableTonConnect: function()
        {
            if (!window.tonConnectUI)
            {
                console.error(`[Uniton Connect] Ton Connect UI is not initialized`);

                return false;
            }

            return true;
        },

        isAvailableTonWeb: function()
        {
            if (!window.tonWeb)
            {
                console.error(`[Uniton Connect] Library 'Ton Web' is not exist`);

                return false;
            }

            return true;
        },

        isInitialized: function()
        {
            if (!tonConnect.isAvailableTonConnect())
            {
                return false;
            }

            if (!tonConnect.isAvailableTonWeb())
            {
                return false;
            }

            return true;
        },

        parseAddress: function(address)
        {
            const correctAddress = UTF8ToString(address);

            const parsedAddress = new window.tonWeb.utils.Address(correctAddress);

            return parsedAddress;
        },

        parseSignMessageSignature: function(
            walletMessage, messageSignFailed)
        {
            let messageEntity = null;

            try
            {
                messageEntity = JSON.parse(walletMessage);
            }
            catch (error)
            {
                var errorPtr = tonConnect.allocString("INVALID_JSON");

                console.error(`[Uniton Connect] Failed to parse sign `+
                    `message object, reasonn: ${error}`);

                {{{ makeDynCall('vi', 'messageSignFailed') }}}(errorPtr);

                _free(errorPtr);

                return;
            }

            const signTypes =
            {
                text:
                {
                    type: "text",
                    text: messageEntity.text,
                    network: messageEntity.network,
                    from: messageEntity.from
                },
                binary:
                {
                    type: "binary",
                    bytes: messageEntity.bytes,
                    network: messageEntity.network,
                    from: messageEntity.from
                },
                cell:
                {
                    type: "cell",
                    schema: messageEntity.schema,
                    cell: messageEntity.cell,
                    network: messageEntity.network,
                    from: messageEntity.from
                }
            };

            const currentType = messageEntity.type;
            const targetSignature = signTypes[currentType];

            if (!targetSignature)
            {
                var invalidTypePtr = tonConnect.allocString("UNSUPPORTED_SIGN_TYPE");

                {{{ makeDynCall('vi', 'messageSignFailed') }}}(invalidTypePtr);

                _free(invalidTypePtr);

                return null;
            }

            return targetSignature;
        },



        init: function(manifestUrl, callback)
        {
            console.log('[Uniton Connect] INIT CALLED - JSLIB IS LOADED!');
            const url = UTF8ToString(manifestUrl);

            window.tonConnectUI = new TON_CONNECT_UI.TonConnectUI(
            {
                manifestUrl: url,
            });

            if (!tonConnect.isAvailableTonConnect())
            {
                {{{ makeDynCall('vi', 'callback') }}}(0);

                return;
            }

            {{{ makeDynCall('vi', 'callback') }}}(1);
        },
        
        initTonWeb: function()
        {
            window.tonWeb = new TonWeb({ apiUrl: 'https://testnet.toncenter.com/api/v2/jsonRPC' });
            console.log('[Uniton Connect] Forced network: TESTNET');
        },

        openModal: async function(callback)
        {
            try
            {
                await window.tonConnectUI.openModal();

                {{{ makeDynCall('vi', 'callback') }}}(1);
            }
            catch (error)
            {
                {{{ makeDynCall('vi', 'callback') }}}(0);
            }
        },

        closeModal: function(callback)
        {
            try
            {
                window.tonConnectUI.closeModal();

                {{{ makeDynCall('vi', 'callback') }}}(1);
            }
            catch (error)
            {
                {{{ makeDynCall('vi', 'callback') }}}(0);
            }
        },

        disconnect: async function(callback)
        {
            try
            {
                await window.tonConnectUI.disconnect();

                const statusPtr = tonConnect.allocString("200");

                {{{ makeDynCall('vi', 'callback') }}}(statusPtr);

                _free(statusPtr);
            }
            catch (error)
            {
                const statusPtr = tonConnect.allocString("500");

                {{{ makeDynCall('vi', 'callback') }}}(statusPtr);

                _free(statusPtr);
            }
        },

        signData: async function(textData, messageSignFailed)
        {
            if (!tonConnect.isInitialized())
            {
                const errorPtr = tonConnect.allocString("NOT_INITIALIZED");
            
                {{{ makeDynCall('vi', 'messageSignFailed') }}}(errorPtr);

                _free(errorPtr);

                return;
            }

            const message = UTF8ToString(textData);

            console.log(`[Uniton Connect] Parsed wallet message for sign: ${message}`);

            const signData = tonConnect.parseSignMessageSignature(message, messageSignFailed);

            if (!signData)
            {
                return;
            }

            console.log(`Final wallet message for sign: ${JSON.stringify(signData)}`);

            try
            {
                const payload = await window.tonConnectUI.signData(signData);

                const signedPayload = JSON.stringify(payload);

                console.log(`[Uniton Connect] Wallet message successfully sign, payload: ${signedPayload}`)
            }
            catch (error)
            {
                const errorDescription = error.message || error;

                console.error(`Failed to sign wallet data, reason: ${errorDescription}`);

                const errorPtr = tonConnect.allocString(errorDescription);

                {{{ makeDynCall('vi', 'messageSignFailed') }}}(errorPtr);

                _free(errorPtr);
            }
        },

        getModalState: function(valueClaimed)
        {
            const stateEntity = window.tonConnectUI.modalState;
            
            const state = JSON.stringify(stateEntity);
            const statePtr = tonConnect.allocString(state);

            console.log(`[Uniton Connect] Current modal state: ${state}`);

            {{{ makeDynCall('vi', 'valueClaimed') }}}(statePtr);

            _free(statePtr);
        },

        subscribeToStatusChanged: function(callback)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            window.unsubscribeFromStatusChange = window.
                tonConnectUI.onStatusChange((wallet) =>
            {
                if (wallet)
                {
                    const walletInfo = JSON.stringify(window.tonConnectUI.account);
                    const walletPtr = tonConnect.allocString(walletInfo);

                    console.log(`[Uniton Connect] Parsed wallet account: ${walletInfo}`);
    
                    {{{ makeDynCall('vi', 'callback') }}}(walletPtr);

                    _free(walletPtr);

                    return;
                }

                const statusPtr = tonConnect.allocString("CONNECT_FAILED");

                {{{ makeDynCall('vi', 'callback') }}}(statusPtr);

                _free(statusPtr);
            });
        },

        unsubscribeFromStatusChanged: function()
        {
            if (window.unsubscribeFromStatusChange)
            {
                window.unsubscribeFromStatusChange();

                window.unsubscribeFromStatusChange = null;

                console.log(`[Uniton Connect] Unsubscribed from 'wallet-status-changed' event`);
            }
        },

        subscribeToModalState: function(modalStateCallback)
        {
            window.unsubsribeFromModalState = window.
                tonConnectUI.onModalStateChange((state) =>
            {
                const stateInfo = JSON.stringify(state);
                const statePtr = tonConnect.allocString(stateInfo);

                console.log(`[Uniton Connect] Claimed 'modal-state-changed' `+
                    `event data, status: ${stateInfo}`);

                {{{ makeDynCall('vi', 'modalStateCallback') }}}(statePtr);

                _free(statePtr);

            });
        },

        unsubscribeFromModalState: function()
        {
            if (window.unsubsribeFromModalState)
            {
                window.unsubsribeFromModalState();

                window.unsubsribeFromModalState = null;

                console.log(`[Uniton Connect] Unsubscribed from 'modal-state-changed' event`);
            }
        },

        subscribeToRestoreConnection: function(callback)
        {
            if (!tonConnect.isInitialized())
            {
                {{{ makeDynCall('vi', 'callback') }}}(0);

                return;
            }

            window.tonConnectUI.connectionRestored.then(restored =>
            {
                if (restored)
                {
                    {{{ makeDynCall('vi', 'callback') }}}(1);

                    return;
                }

                {{{ makeDynCall('vi', 'callback') }}}(0);
            });
        },

        subscribeToTransactionEvents: function(
            successCallback, errorCallback)
        {
            const signedHandler = (event) =>
            {
                const data = event.detail;

                console.log(`[Uniton Connect] Transaction signed:`, data);

                const signedData = JSON.stringify(data);
                const signedPtr = tonConnect.allocString(signedData);

                {{{ makeDynCall('vi', 'successCallback') }}}(signedPtr);

                _free(signedPtr);
            };

            const failedHandler = (event) =>
            {
                const data = event.detail;

                console.warn(`[Uniton Connect] Transaction signing failed:`, data);

                const failedData = JSON.stringify(data);
                const failedPtr = tonConnect.allocString(failedData);

                {{{ makeDynCall('vi', 'errorCallback') }}}(failedPtr);

                _free(failedPtr);
            };

            window.addEventListener('ton-connect-ui-transaction-signed', signedHandler);
            window.addEventListener('ton-connect-ui-transaction-signing-failed', failedHandler);

            window._tonConnectTransactionSignedHandler = signedHandler;
            window._tonConnectTransactionFailedHandler = failedHandler;

            console.log('[Uniton Connect] Subscribed to transaction events');
        },

        unsubscribeFromTransactionEvents: function()
        {
            if (window._tonConnectTransactionSignedHandler)
            {
                window.removeEventListener('ton-connect-ui-transaction-signed',
                    window._tonConnectTransactionSignedHandler);
                
                delete window._tonConnectTransactionSignedHandler;

                console.log(`[Uniton Connect] Unsubscribed from 'transaction-signed' event`);
            }

            if (window._tonConnectTransactionFailedHandler)
            {
                window.removeEventListener('ton-connect-ui-transaction-signing-failed',
                    window._tonConnectTransactionFailedHandler);

                delete window._tonConnectTransactionSignedHandler;

                console.log(`[Uniton Connect] Unsubsribed from 'transaction-signing-failed' event`);
            }
        },

        subscribeToWalletMessageSigned: function(
            successSign, failedSign)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            const signedHandler = (event) =>
            {
                const data = event.detail.signed_data;
                
                console.log(`[Uniton Connect] Wallet message signed`, data);

                const signedData = JSON.stringify(data);
                const signedPtr = tonConnect.allocString(signedData);

                {{{ makeDynCall('vi', 'successSign') }}}(signedPtr);

                _free(signedPtr);
            };

            const failedHandler = (event) =>
            {
                const data = event.detail;

                console.warn(`[Uniton Connect] Failed to sign wallet message`, data);

                const failedData = JSON.stringify(data);
                const failedPtr = tonConnect.allocString(failedData);

                {{{ makeDynCall('vi', 'failedSign') }}}(failedPtr);

                _free(failedPtr);
            };

            window.addEventListener('ton-connect-ui-sign-data-request-completed', signedHandler);
            window.addEventListener('ton-connect-ui-sign-data-request-failed', failedHandler);

            window._tonConnectMessageSignedHandler = signedHandler;
            window._tonConnectMessageSignFailedHandler = failedHandler;

            console.log('[Uniton Connect] Subscribed to message sign events');
        },

        unsubscribeFromWalletMessageSigned: function()
        {
            if (window._tonConnectMessageSignedHandler)
            {
                window.removeEventListener('ton-connect-ui-sign-data-request-completed',
                    window._tonConnectMessageSignedHandler);
                
                delete window._tonConnectMessageSignedHandler;

                console.log(`[Uniton Connect] Unsubscribed from 'wallet-message-signed' event`);
            }

            if (window._tonConnectMessageSignFailedHandler)
            {
                window.removeEventListener('ton-connect-ui-sign-data-request-failed',
                    window._tonConnectMessageSignFailedHandler);

                delete window._tonConnectMessageSignFailedHandler;

                console.log(`[Uniton Connect] Unsubsribed from 'wallet-message-sign-failed' event`);
            }
        },

        sendAsssetsTransaction: async function(itemAddress,
            gasFeeAmount, payload, callback)
        {
            if (!tonConnect.isInitialized())
            {
                const errorMessage = tonConnect.allocString("SDK is not initialized");

                {{{ makeDynCall('vi', 'callback') }}}(errorMessage);

                _free(errorMessage);

                return;
            }

            const tonWeb = window.tonWeb;

            const senderOrNftAddress = UTF8ToString(itemAddress);
            const gasFee = UTF8ToString(gasFeeAmount);
            const transactionPayload = UTF8ToString(payload);

            console.log(`[Uniton Connect] Parsed assets transaction `+
                `payload: ${transactionPayload}`);

            try
            {
                transactionData =
                {
                    validUntil: Math.floor(Date.now() / 1000) + 360,
                    messages:
                    [
                        {  
                            address: senderOrNftAddress, 
                            amount: gasFee,
                            payload: transactionPayload
                        }
                    ]
                };

                console.log(`[Uniton Connect] Parsed assets transaction `+
                    `data: ${JSON.stringify(transactionData)}`);

                const result = await window.tonConnectUI.sendTransaction(transactionData, 
                {
                    modals: ['before', 'success', 'error'],
                    notifications: ['before', 'success', 'error']
                });
            
                if (!result || !result.boc)
                {
                    const emptyPtr = tonConnect.allocString("EMPTY_BOC");

                    console.error(`[Uniton Connect] No BOC returned from assets transaction`);

                    {{{ makeDynCall('vi', 'callback') }}}(emptyPtr);

                    _free(emptyPtr);

                    return;
                }
                
                let claimedBoc = result.boc;

                const hashBase64 = await tonConnect.convertBocToHashBase64(claimedBoc);
                const hashPtr = tonConnect.allocString(hashBase64);

                console.log(`[Uniton Connect] Parsed assets transaction `+
                    `hash: ${JSON.stringify(hashBase64)}`);

                {{{ makeDynCall('vi', 'callback') }}}(hashPtr);

                _free(hashPtr);
            }
            catch (error)
            {
                const errorPtr = tonConnect.allocString("");
            
                {{{ makeDynCall('vi', 'callback') }}}(errorPtr);

                _free(errorPtr);
            }
        },

        sendTonTransaction: async function(nanoInTon,
            recipientAddress, message, callback) 
        {
            if (!tonConnect.isInitialized()) 
            {
                const nullPtr = tonConnect.allocString("null");

               {{{ makeDynCall('vi', 'callback') }}}(nullPtr);

                _free(nullPtr);

                return;
            }

            const tonWeb = window.tonWeb;

            const nanotons = UTF8ToString(nanoInTon);
            const address = UTF8ToString(recipientAddress);
            const payloadMessage = UTF8ToString(message);

            console.log("[Uniton Debug] Payload inputs:", {
                nanotons,
                address,
                payloadMessage
            });
            const transactionData = await tonConnect.getTonTransactionPayload(
                nanotons, address, payloadMessage);

            try
            {
                const result = await window.tonConnectUI.sendTransaction(transactionData, 
                {
                    modals: ['before', 'success', 'error'],
                    notifications: ['before', 'success', 'error']
                });
            
                if (!result || !result.boc)
                {
                    const emptyPtr = tonConnect.allocString("EMPTY_BOC");

                    console.error(`[Uniton Connect] No BOC returned from toncoin transaction`);

                    {{{ makeDynCall('vi', 'callback') }}}(emptyPtr);

                    _free(emptyPtr);

                    return;
                }
                
                let claimedBoc = result.boc;

                const hashBase64 = await tonConnect.convertBocToHashBase64(claimedBoc);
                const hashPtr = tonConnect.allocString(hashBase64);

                console.log(`[Uniton Connect] Parsed toncoin transaction `+
                    `hash: ${JSON.stringify(hashBase64)}`);

                {{{ makeDynCall('vi', 'callback') }}}(hashPtr);

                _free(hashPtr);
            }
            catch (error)
            {
                console.error("[Uniton Debug] Transaction error:", error);
                const errorPtr = tonConnect.allocString("");

                {{{ makeDynCall('vi', 'callback') }}}(errorPtr);

                _free(errorPtr);
            }
        },

        SendSmartContractTx: async function(nanoTonPtr, methodPtr, jsonParamsPtr, callback) 
        {
            if (!tonConnect.isInitialized()) 
            {
                const nullPtr = tonConnect.allocString("null");

               {{{ makeDynCall('vi', 'callback') }}}(nullPtr);

                _free(nullPtr);

                return;
            }

            const amount = UTF8ToString(nanoTonPtr);
            const toAddr = tonConnect.CONTRACT_ADDRESS; //smartcontract
            const method = UTF8ToString(methodPtr);
            const jsonParams = UTF8ToString(jsonParamsPtr);
        
            const tonWeb = window.tonWeb; 
            let payloadBase64 = "";

            console.log("[Uniton Debug] Payload inputs:", {
                amount,
                toAddr,
                method,
                jsonParams
            });

            try {
                const params = jsonParams ? JSON.parse(jsonParams) : {};
                const cell = new tonWeb.boc.Cell();
            
                switch (method) {
                    case "claim_reward":
                        cell.bits.writeUint(0x27BB9A66, 32); 
                        cell.bits.writeCoins(BigInt(params.amountReward))
                        break;

                    case "buy_heart":
                        cell.bits.writeUint(0xC63CDE42, 32);
                        cell.bits.writeUint(params.qty, 32);
                        break;
                    
                    case "buy_laser":
                        cell.bits.writeUint(0xC3B889DA, 32);
                        cell.bits.writeUint(params.qty, 32);
                        break;
                    
                    case "use_heart":
                        cell.bits.writeUint(0xD5E8C916, 32);
                        cell.bits.writeUint(params.qty, 32);
                        break;
                    
                    case "use_laser":
                        cell.bits.writeUint(0x86821106, 32);
                        cell.bits.writeUint(params.qty, 32);
                        break;
                    
                    case "buy_upgrade":
                        cell.bits.writeUint(0x7AE5EF0, 32);
                        cell.bits.writeUint(params.upgradeType, 32);
                        break;
                    
                    case "heart_reward":
                        cell.bits.writeUint(0x5B56DDC9, 32);
                        break;

                    default:
                        console.error("Unknown method: " + method);
                        return;
                }

                payloadBase64 = tonWeb.utils.bytesToBase64(await cell.toBoc());

            } catch (e) {
                console.error("[Uniton Connect] Error building payload:", e);

                const errorPtr = tonConnect.allocString("");

                {{{ makeDynCall('vi', 'callback') }}}(errorPtr);
                _free(errorPtr);

                return;
            }

            // --- SEND TRANSACTION ---
            const transaction = {
                validUntil: Math.floor(Date.now() / 1000) + 120, // 2 min
                messages: [{
                    address: toAddr,
                    amount: amount,
                    payload: payloadBase64
                }]
            };

            try
            {
                const result = await window.tonConnectUI.sendTransaction(transaction, 
                {
                    modals: ['before', 'success', 'error'],
                    notifications: ['before', 'success', 'error']
                });
            
                if (!result || !result.boc)
                {
                    const emptyPtr = tonConnect.allocString("EMPTY_BOC");

                    console.error(`[Uniton Connect] No BOC returned from toncoin transaction`);

                    {{{ makeDynCall('vi', 'callback') }}}(emptyPtr);

                    _free(emptyPtr);

                    return;
                }
                
                let claimedBoc = result.boc;

                const hashBase64 = await tonConnect.convertBocToHashBase64(claimedBoc);
                const hashPtr = tonConnect.allocString(hashBase64);

                console.log(`[Uniton Connect] Parsed toncoin transaction `+
                    `hash: ${JSON.stringify(hashBase64)}`);

                {{{ makeDynCall('vi', 'callback') }}}(hashPtr);

                _free(hashPtr);
            }
            catch (error)
            {
                console.error("[Uniton Debug] Transaction error:", error);
                const errorPtr = tonConnect.allocString("");

                {{{ makeDynCall('vi', 'callback') }}}(errorPtr);

                _free(errorPtr);
            }
        },

        getTonTransactionPayload: async function(
            nanoInTon, recipientAddress, message)
        {
            const tonWeb = window.tonWeb;
            var transactionData;

            if (!message || message === "CLEAR")
            {
                transactionData =
                {
                    validUntil: Math.floor(Date.now() / 1000) + 60,
                    messages:
                    [
                        {  
                            address: recipientAddress, 
                            amount: nanoInTon
                        }
                    ]
                };

                return transactionData;
            }

            let cellBuilder = new tonWeb.boc.Cell();

            //cellBuilder.bits.writeUint(0, 32);
            cellBuilder.bits.writeUint(0x4a25ce37, 32); 
            //cellBuilder.bits.writeString(message);
                
            let payload = tonWeb.utils.bytesToBase64(await cellBuilder.toBoc());

            transactionData =
            {
                validUntil: Math.floor(Date.now() / 1000) + 60,
                messages:
                [
                    {  
                        address: recipientAddress, 
                        amount: nanoInTon,
                        payload: payload
                    },
                ]
            };

            return transactionData;
        },
        
        GetPlayerTotalScore: function (playerAddressPtr, callback) 
        {
            console.log('[Uniton Connect] GetPlayerTotalScore called');
    
            var playerAddress = UTF8ToString(playerAddressPtr);
            var tonWeb = window.tonWeb;

            if (!tonWeb) 
            {
                console.error('[Uniton Connect] TonWeb not initialized!');
                 errPtr = tonConnect.allocString('0');
                {{{ makeDynCall('vi', 'callback') }}}(errPtr);
                _free(errPtr);
                return;
            }

            console.log('[Uniton Connect] Calling get_score for:', playerAddress);

            

            // Create address cell
            var addressObj = new tonWeb.utils.Address(playerAddress);
            var cell = new tonWeb.boc.Cell();
            cell.bits.writeAddress(addressObj);
    
            cell.toBoc(false).then(function(bocBytes) {
                var bocBase64 = tonWeb.utils.bytesToBase64(bocBytes);
                console.log('[Uniton Connect] Address BOC:', bocBase64);
        
                var apiUrl = 'https://testnet.toncenter.com/api/v2/runGetMethod';
                var requestBody = JSON.stringify({
                    address: tonConnect.CONTRACT_ADDRESS,
                    method: "get_score",
                    stack: [["tvm.Slice", bocBase64]]
                });

                console.log('[Uniton Connect] Request:', requestBody);

                return fetch(apiUrl, {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: requestBody
                });
            })
            .then(function(response) {
                return response.json();
            })
            .then(function(data) {
                console.log('[Uniton Connect] API Response:', data);
                console.log('[Uniton Connect] Exit code:', data.result ? data.result.exit_code : 'N/A');
        
                var score = 0;
                if (data.ok && data.result && data.result.exit_code === 0) {
                if (data.result.stack && data.result.stack.length > 0) {
                        var stackItem = data.result.stack[0];
                        console.log('[Uniton Connect] Stack item:', stackItem);
                
                        if (stackItem[0] === 'num') {
                            score = parseInt(stackItem[1], 16);
                            console.log('[Uniton Connect] Score (decimal):', score);
                        }
                }
                } else {
                    console.warn('[Uniton Connect] Failed - exit code:', data.result ? data.result.exit_code : 'N/A');
                }
        
                var scoreStr = score.toString();
                var strPtr = tonConnect.allocString(scoreStr);

                console.log('[Uniton Connect] Returning to Unity:', scoreStr);
                {{{ makeDynCall('vi', 'callback') }}}(strPtr);
                _free(strPtr);
            })
            .catch(function(error) {
                console.error('[Uniton Connect] Error:', error);
                var errPtr = tonConnect.allocString('0');
                {{{ makeDynCall('vi', 'callback') }}}(errPtr);
                _free(errPtr);
            });
        },

        GetPlayerItemCount: function (methodNamePtr, playerAddressPtr, callback) 
        {
            console.log('[Uniton Connect] GetPlayerItemCount called');
    
            var methodName = UTF8ToString(methodNamePtr);
            var playerAddress = UTF8ToString(playerAddressPtr);
            var tonWeb = window.tonWeb;

            if (!tonWeb) 
            {
                console.error('[Uniton Connect] TonWeb not initialized!');
                var failResult = '0'
                if (methodName === "get_player_info") {
                    failResult = JSON.stringify({
                        heart: 0, laser: 0, lastTimeReceiveHeart: 0, 
                        remainingHeartCooldown: 0, levelHeart: 1, levelDig: 1, levelOre: 1
                    });
                }

                errPtr = tonConnect.allocString(failResult);
                {{{ makeDynCall('vi', 'callback') }}}(errPtr);
                _free(errPtr);
                return;
            }
            console.log('[Uniton Connect] Calling get_info for:', playerAddress);

            var retryWithBackoff = function(fn, maxRetries, baseDelay) {
                var attempt = 0;
        
                var tryRequest = function() {
                    attempt++;
                    console.log('[Uniton Connect] Attempt ' + attempt + '/' + maxRetries + ': ' + methodName);
            
                    return fn().catch(function(error) {
                        if (attempt >= maxRetries) {
                            console.error('[Uniton Connect] All ' + maxRetries + ' attempts failed: ' + methodName);
                            throw error;
                        }
                
                        // Exponential backoff: 1s, 2s, 4s
                        var delay = baseDelay * Math.pow(2, attempt - 1);
                        console.log('[Uniton Connect] Request failed, retrying in ' + delay + 'ms...');
                        console.log('[Uniton Connect] Error was:', error.message || error);
                
                        return new Promise(function(resolve) {
                            setTimeout(function() {
                                resolve(tryRequest());
                            }, delay);
                        });
                    });
                };
        
                return tryRequest();
            };

            //add time out for fetch
            var fetchWithTimeout = function(url, options, timeout) {
                return Promise.race([
                    fetch(url, options),
                    new Promise(function(_, reject) {
                        setTimeout(function() {
                            reject(new Error('Request timeout after ' + timeout + 'ms'));
                        }, timeout);
                    })
                ]);
            };

            // Create address cell
            var addressObj = new tonWeb.utils.Address(playerAddress);
            var cell = new tonWeb.boc.Cell();
            cell.bits.writeAddress(addressObj);
    
            cell.toBoc(false).then(function(bocBytes) {
                var bocBase64 = tonWeb.utils.bytesToBase64(bocBytes);
                console.log('[Uniton Connect] Address BOC:', bocBase64);
        
                var apiUrl = 'https://testnet.toncenter.com/api/v2/runGetMethod';
                var requestBody = JSON.stringify({
                    address: tonConnect.CONTRACT_ADDRESS,
                    method: methodName,
                    stack: [["tvm.Slice", bocBase64]]
                });

                console.log('[Uniton Connect] Request:', requestBody);

                // return fetch(apiUrl, {
                //     method: 'POST',
                //     headers: {'Content-Type': 'application/json'},
                //     body: requestBody
                // });

                var makeRequest = function() {
                    return fetchWithTimeout(apiUrl, {
                        method: 'POST',
                        headers: {'Content-Type': 'application/json'},
                        body: requestBody
                    }, 10000) // 10 second timeout
                    .then(function(response) {
                        if (!response.ok) {
                            throw new Error('HTTP error! status: ' + response.status);
                        }
                        return response.json();
                    });
                };
        
                // retry with 3 attempts, delay start with 1000ms
                return retryWithBackoff(makeRequest, 3, 1000); //makeRequest again

            })
            // .then(function(response) {
            //     return response.json();
            // })
            .then(function(data) {
                console.log('[Uniton Connect] API Response:', data);
                console.log('[Uniton Connect] Exit code:', data.result ? data.result.exit_code : 'N/A');

                var item = 0;
                var itemJson = JSON.stringify({
                    heart: 0, laser: 0, lastTimeReceiveHeart: 0, 
                    remainingHeartCooldown: 0, levelHeart: 1, levelDig: 1, levelOre: 1
                });

                if (data.ok && data.result && data.result.exit_code === 0) 
                {
                    if (data.result.stack && data.result.stack.length > 0) {
                        if(methodName == "get_player_info")
                        {
                            var stack = data.result.stack;
                            var info = {
                                heart: 0,
                                laser: 0,
                                lastTimeReceiveHeart: 0,
                                remainingHeartCooldown: 0,
                                levelHeart: 0,
                                levelDig: 0,
                                levelOre: 0
                            };

                            var getVal = function(i) { 
                                return (stack.length > i && stack[i][0] === 'num') ? parseInt(stack[i][1], 16) : 0; 
                            };

                            info.heart = getVal(1);
                            info.laser = getVal(2);
                            info.lastTimeReceiveHeart = getVal(3);
                            info.remainingHeartCooldown = getVal(4);
                            info.levelHeart = getVal(5);
                            info.levelDig = getVal(6);
                            info.levelOre = getVal(7);

                            itemJson = JSON.stringify(info);
                        }
                        else
                        {
                            var stackItem = data.result.stack[0];
                            console.log('[Uniton Connect] Stack item:', stackItem);
                
                            if (stackItem[0] === 'num') {
                                item = parseInt(stackItem[1], 16);
                                console.log('[Uniton Connect] Item (decimal):', item);
                            }
                        }
                    }
                } 
                else 
                {
                    console.warn('[Uniton Connect] Failed - exit code:', data.result ? data.result.exit_code : 'N/A');
                }
        
                var strPtr;
                if(methodName=="get_player_info")
                {
                    strPtr = tonConnect.allocString(itemJson);
                }
                else
                {
                    var itemStr = item.toString();
                    strPtr = tonConnect.allocString(itemStr);
                    
                    console.log('[Uniton Connect] Returning to Unity:', itemStr);
                }

                {{{ makeDynCall('vi', 'callback') }}}(strPtr);
                _free(strPtr);
            })
            .catch(function(error) {
                console.error('[Uniton Connect] Error:', error);

                var errPtr;
                if(methodName=="get_player_info")
                {  
                    errPtr = tonConnect.allocString(JSON.stringify({
                        heart: 0, laser: 0, lastTimeReceiveHeart: 0, 
                        remainingHeartCooldown: 0, levelHeart: 1, levelDig: 1, levelOre: 1
                    }));
                    
                }
                else   
                {
                    errPtr = tonConnect.allocString('0');
                }

                {{{ makeDynCall('vi', 'callback') }}}(errPtr);
                _free(errPtr);
            });
        },

        GetGameData: function (methodNamePtr, callback) 
        {
            console.log('[Uniton Connect] GetGameData called');
    
            var methodName = UTF8ToString(methodNamePtr);
            var tonWeb = window.tonWeb;

            if (!tonWeb) 
            {
                console.error('[Uniton Connect] TonWeb not initialized!');
                 errPtr = tonConnect.allocString('0');
                {{{ makeDynCall('vi', 'callback') }}}(errPtr);
                _free(errPtr);
                return;
            }

            var retryWithBackoff = function(fn, maxRetries, baseDelay) {
                var attempt = 0;
        
                var tryRequest = function() {
                    attempt++;
                    console.log('[Uniton Connect] Attempt ' + attempt + '/' + maxRetries);
            
                    return fn().catch(function(error) {
                        if (attempt >= maxRetries) {
                            console.error('[Uniton Connect] All ' + maxRetries + ' attempts failed');
                            throw error;
                        }
                
                        // Exponential backoff: 1s, 2s, 4s
                        var delay = baseDelay * Math.pow(2, attempt - 1);
                        console.log('[Uniton Connect] Request failed, retrying in ' + delay + 'ms...');
                        console.log('[Uniton Connect] Error was:', error.message || error);
                
                        return new Promise(function(resolve) {
                            setTimeout(function() {
                                resolve(tryRequest());
                            }, delay);
                        });
                    });
                };
        
                return tryRequest();
            };

            //add time out for fetch
            var fetchWithTimeout = function(url, options, timeout) {
                return Promise.race([
                    fetch(url, options),
                    new Promise(function(_, reject) {
                        setTimeout(function() {
                            reject(new Error('Request timeout after ' + timeout + 'ms'));
                        }, timeout);
                    })
                ]);
            };

            var apiUrl = 'https://testnet.toncenter.com/api/v2/runGetMethod';
        
            var requestBody = JSON.stringify({
                address: tonConnect.CONTRACT_ADDRESS,
                method: methodName,
                stack: [] //no parameter
            });

            console.log('[Uniton Connect] Request Price:', requestBody);

            var makeRequest = function() {
                return fetchWithTimeout(apiUrl, {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: requestBody
                }, 10000)
                .then(function(response) {
                    if (!response.ok) throw new Error('HTTP ' + response.status);
                    return response.json();
                });
            };

            retryWithBackoff(makeRequest, 3, 1000)
            .then(function(data) {
                console.log('[Uniton Connect] Price Response:', data);

                var price = 0;
                if (data.ok && data.result && data.result.exit_code === 0) {
                    if (data.result.stack && data.result.stack.length > 0) {
                        var stackItem = data.result.stack[0];
                        if (stackItem[0] === 'num') {
                            price = parseInt(stackItem[1], 16);
                        }
                    }
                } else {
                    console.warn('[Uniton Connect] Failed exit code:', data.result ? data.result.exit_code : 'N/A');
                }

                var priceStr = price.toString();
                var strPtr = tonConnect.allocString(priceStr);
                {{{ makeDynCall('vi', 'callback') }}}(strPtr);
                _free(strPtr);
            })
            .catch(function(error) {
                console.error('[Uniton Connect] Error:', error);
                var errPtr = tonConnect.allocString('0');
                {{{ makeDynCall('vi', 'callback') }}}(errPtr);
                _free(errPtr);
            });
        },

        convertBocToHashBase64: async function(claimedBoc)
        {
            const bocBytes = tonWeb.utils.base64ToBytes(claimedBoc);

            const bocCellBytes = await tonWeb.boc.Cell.oneFromBoc(bocBytes).hash();
            const hashBase64 = tonWeb.utils.bytesToBase64(bocCellBytes);

            return hashBase64;
        },

        toBounceable: function(address, valueClaimed)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            const parsedAddress = tonConnect.parseAddress(address);
            //const bouceableAddress = parsedAddress.toString(true, true, true, false);
            const bouceableAddress = parsedAddress.toString(true, true, true, true);

            console.log(`[Uniton Connect] Address ${parsedAddress} converted `+
                `to bounceable format: ${bouceableAddress}`);

            const addressPtr = tonConnect.allocString(bouceableAddress);

            {{{ makeDynCall('vi', 'valueClaimed') }}}(addressPtr);

            _free(addressPtr);
        },

        toNonBounceable: function(address, valueClaimed)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            const parsedAddress = tonConnect.parseAddress(address);
            const nonBouceableAddress = parsedAddress.toString(true, true, false, false);

            console.log(`[Uniton Connect] Address ${parsedAddress} converted to `+
                `non bounceable format: ${nonBouceableAddress}`);

            const addressPtr = tonConnect.allocString(nonBouceableAddress);

            {{{ makeDynCall('vi', 'valueClaimed') }}}(addressPtr);

            _free(addressPtr);
        },

        toHex: function(address, valueClaimed)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            const parsedAddress = tonConnect.parseAddress(address);
            const hexAddress = parsedAddress.toString(false);

            console.log(`[Uniton Connect] Address ${parsedAddress} `+
                `converted to hex/raw format: ${hexAddress}`);

            const addressPtr = tonConnect.allocString(hexAddress);

            {{{ makeDynCall('vi', 'valueClaimed') }}}(addressPtr);

            _free(addressPtr);
        },

        isUserFriendly: function(address)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            return tonConnect.parseAddress(address).isUserFriendly;
        },

        isBounceable: function(address)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            return tonConnect.parseAddress(address).isBounceable;
        },

        isTestOnly: function(address)
        {
            if (!tonConnect.isInitialized())
            {
                return;
            }

            return tonConnect.parseAddress(address).isTestOnly;
        }
    },

    Init: function(manifestUrl, callback)
    {
        tonConnect.init(manifestUrl, callback);
    },

    InitTonWeb: function()
    {
        tonConnect.initTonWeb();
    },

    OpenModal: function(callback)
    {
        tonConnect.openModal(callback);
    },

    CloseModal: function(callback)
    {
        tonConnect.closeModal(callback);
    },

    Disconnect: function(callback)
    {
        tonConnect.disconnect(callback);
    },

    SubscribeToModalState: function(callback)
    {
        tonConnect.subscribeToModalState(callback);
    },

    UnsubscribeFromModalState: function()
    {
        tonConnect.unsubscribeFromModalState();
    },

    SubscribeToStatusChange: function(callback)
    {
        tonConnect.subscribeToStatusChanged(callback);
    },

    UnsubscribeFromStatusChange: function() 
    {
        tonConnect.unsubscribeFromStatusChanged();
    },

    SubscribeToRestoreConnection: function(callback)
    {
        tonConnect.subscribeToRestoreConnection(callback);
    },

    SubscribeToTransactionEvents: function(
        successCallback, errorCallback)
    {
        tonConnect.subscribeToTransactionEvents(
            successCallback, errorCallback);
    },

    UnsubscribeFromTransactionEvents: function()
    {
        tonConnect.unsubscribeFromTransactionEvents();
    },

    SubscribeToWalletMessageSigned: function(
        successCallback, errorCallback)
    {
        tonConnect.subscribeToWalletMessageSigned(
            successCallback, errorCallback);
    },

    UnsubscribeFromWalletMessageSigned: function()
    {
        tonConnect.unsubscribeFromWalletMessageSigned();
    },

    SendTonTransaction: function(
        nanoInTon, recipientAddress, callback)
    {
        tonConnect.sendTonTransaction(nanoInTon,
            recipientAddress, "CLEAR", callback);
    },

    GetPlayerScore: function(playerAddress, callback)
    {
        console.log(`[Uniton Connect] Get player score called`);
        tonConnect.GetPlayerTotalScore(playerAddress, callback);
    },

    GetPlayerStat: function(methodName, playerAddress, callback)
    {
        console.log(`[Uniton Connect] Get player stat called`);
        tonConnect.GetPlayerItemCount(methodName, playerAddress, callback);
    },

    GetGameInfo: function(methodName, callback)
    {
        console.log(`[Uniton Connect] Get game info called`);
        tonConnect.GetGameData(methodName, callback);
    },

    SendTonTransactionWithMessage: function(nanoInTon,
        recipientAddress, message, callback)
    {
        tonConnect.sendTonTransaction(nanoInTon,
            recipientAddress, message, callback);
    },

    SendTonSmartContractTx: function(nanoTonPtr, methodPtr, jsonParamsPtr, callback)
    {
        tonConnect.SendSmartContractTx(nanoTonPtr, methodPtr, jsonParamsPtr, callback);
    },

    SendTransactionWithPayload: function(targetAddress,
        gasFee, payload, callback)
    {
        tonConnect.sendAsssetsTransaction(
            targetAddress, gasFee, payload, callback);
    },

    SignData: function(message, messageSignFailed)
    {
        tonConnect.signData(message, messageSignFailed);
    },

    GetModalState: function(callback)
    {
        tonConnect.getModalState(callback);
    },

    ToBounceableAddress: function(address, valueClaimed)
    {
        tonConnect.toBounceable(address, valueClaimed);
    },

    ToNonBounceableAddress: function(address, valueClaimed)
    {
        tonConnect.toNonBounceable(address, valueClaimed);
    },

    ToHexAddress: function(address, valueClaimed)
    {
        tonConnect.toHex(address, valueClaimed);
    },

    IsUserFriendlyAddress: function(address)
    {
        return tonConnect.isUserFriendly(address);
    },

    IsBounceableAddress: function(address)
    {
        return tonConnect.isBounceable(address);
    },

    IsTestnetAddress: function(address)
    {
        return tonConnect.isTestOnly(address);
    }
};

autoAddDeps(tonConnectBridge, '$tonConnect');
mergeInto(LibraryManager.library, tonConnectBridge);