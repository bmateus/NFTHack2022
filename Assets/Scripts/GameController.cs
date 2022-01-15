using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
using System;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    MoralisUser user;

    [SerializeField]
    GameObject player;

    [SerializeField]
    TMP_Text address;

    // Start is called before the first frame update
    async void Start()
    {
        try
        {
            user = await MoralisInterface.GetUserAsync();

            if (user != null)
            {
                string addr = user.authData["moralisEth"]["id"].ToString();
                Debug.Log($"Got user {addr}");
                address.text = string.Format("{0}...{1}", addr.Substring(0, 6), addr.Substring(addr.Length - 3, 3));

                //doesn't work? throws an API Exception
                //Ens result = MoralisInterface.GetClient().Web3Api.Resolve.ResolveAddress(addr); 
                //address.text = result.Name;

                //What is this supposed to do?
                //Debug.Log($"Eth Address: {MoralisInterface.GetClient().EthAddress}");


                //Get the adventurers owned by this player
                var adventurerContract = "0x3adEBD2D62841c9e1110a84a305586861cDEf8cb".ToLower();
                var collection = MoralisInterface.GetClient().Web3Api.Account.GetNFTsForContract(addr.ToLower(), adventurerContract, ChainList.polygon);
                foreach(var nft in collection.Result)
                {
                    var metadata = MoralisInterface.GetClient().Web3Api.Token.GetTokenIdMetadata(adventurerContract, nft.TokenId, ChainList.polygon);
                    Debug.Log($"Got {nft.TokenId}");
                }

            }
            else
            {
                Debug.Log("Couldn't get user!");
                //Send back to startup menu?
            }

            //Check if user has the required character NFT in their wallet


        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
            // go back to startup?
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
