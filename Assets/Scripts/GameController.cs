using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class QueryEnsResponse
{
    public string name;
}

public class Adventurer
{
    public AdventurerMetadata metadata;

    public string Name => metadata.name;

    public AdventurerAttribute[] Traits => metadata.attributes;


    public IEnumerator LoadImage(RawImage target)
    {
        //Load an image from ipfs and assign it to the target;

        var hash = metadata.image.Replace("ipfs://", string.Empty);
        //use the moralis IPFS gateway
        //var webRequest = UnityWebRequestTexture.GetTexture($"https://ipfs.moralis.io:2053/ipfs/{hash}.png");
        //moralis IPFS gateway is crapping out... maybe pinata instead?
        var webRequest = UnityWebRequestTexture.GetTexture($"https://gateway.pinata.cloud/ipfs/{hash}.png");
        //oh damn... forgot to add ".png" to the images in the metadata :(

        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error getting portrait:{webRequest.error}");
        }
        else
        {
            var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture as Texture2D;
            target.texture = texture;
        }
    }
}

[Serializable]
public class AdventurerMetadata
{
    public string dna;
    public string name;
    public string description;
    public string image;
    public int edition;
    public long date;
    public AdventurerAttribute[] attributes;
}

[Serializable]
public class AdventurerAttribute
{
    public string trait_type;
    public string value;
}

public class GameController : MonoBehaviour
{
    MoralisUser user;

    [SerializeField]
    GameObject player;

    [SerializeField]
    TMP_Text address;

    [SerializeField]
    UIPopup purchaseAdventurerPopup;

    [SerializeField]
    AdventurerPopup adventurerPopup;

    List<Adventurer> adventurers = new List<Adventurer>();

    IEnumerator GetENS(string addr)
    {
        var request = UnityWebRequest.Get($"https://deep-index.moralis.io/api/v2/resolve/{addr}/reverse");
        request.SetRequestHeader("X-API-KEY", "jqf3JOk3t1b9XI6FjmpJwLGVUy3FLQw0SfQzoZiRqqU8RYThU5qWmUt3CGp0Tk9u");
        yield return request.SendWebRequest();
        if ( request.result != UnityWebRequest.Result.Success )
        {
            Debug.Log($"Error getting ENS: {request.error}");
        }
        else
        {
            Debug.Log($"Got ENS respose: {request.downloadHandler.text}");
            var response = JsonUtility.FromJson<QueryEnsResponse>(request.downloadHandler.text);
            address.text = response.name;
        }
    }

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

                StartCoroutine(GetENS(addr));

                //What is this supposed to do?
                //Debug.Log($"Eth Address: {MoralisInterface.GetClient().EthAddress}");


                //Get the adventurers owned by this player
                var adventurerContract = "0xF7eF54F931ccBdbad5e6Bf69badFD5af3C202848".ToLower();
                var collection = MoralisInterface.GetClient().Web3Api.Account.GetNFTsForContract(addr.ToLower(), adventurerContract, ChainList.polygon);
                foreach (var nft in collection.Result)
                {
                    var tokenMetadata = MoralisInterface.GetClient().Web3Api.Token.GetTokenIdMetadata(adventurerContract, nft.TokenId, ChainList.polygon);
                    Debug.Log($"Got {nft.TokenId}");

                    adventurers.Add(new Adventurer() {
                        metadata = JsonUtility.FromJson<AdventurerMetadata>(tokenMetadata.Metadata)
                    });
                }

            }
            else
            {
                Debug.Log("Couldn't get user!");
                //Send back to startup menu?
            }

            //Check if user has the required character NFT in their wallet
            if (adventurers.Count < 1)
            {
                purchaseAdventurerPopup.Show();
            }
            else
            {
                adventurerPopup.Init(adventurers);
                adventurerPopup.Show();
            }


        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
            // go back to startup?
        }
    }

    public void EnterDungeon()
    {
        SceneManager.LoadScene("Dungeon");
    }
}
