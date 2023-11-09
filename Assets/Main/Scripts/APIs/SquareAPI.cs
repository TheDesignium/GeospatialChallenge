using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

using TMPro;

public class SquareAPI : MonoBehaviour
{

	public GeospatialController geo;
	public Geo customgeo;
	public LoadKML kml;

	public Animator userInfoAnimator;

	public GameObject mapObject;
	public GameObject rewardObject;
	public GameObject loadingObject;

	public Image rewardImage;
	public TMP_Text rewardTextmain;
	public TMP_Text rewardTextearnings;
	public TMP_Text rewardText;
	public TMP_Text rewardTextTwo;

	public TMP_Text usernameText;
	public TMP_Text userIDText;
	public TMP_Text userCardText;
	public TMP_Text userCardGanText;
	public TMP_Text userBalanceText;

	public Color rewardColour;
	public Color textColour;

    public string baseUrl = "https://connect.squareupsandbox.com";
    public string customerEndpoint = "/v2/customers";
    public string giftCardEndpoint = "/v2/gift-cards";
    private const string linkGiftCardEndpoint = "/v2/gift-cards/";
    private const string createGiftCardActivityEndpoint = "/v2/gift-cards/activities";

    public string accessToken = "EAAAEJ79YmaI12YpZaINn9PBAsuBUDOMdfAXdW8V7W6Dgj3SgmqGyzTDTDA63FF2"; // You'll need your Square access token.

    public string debugname;
    public string debugID;
    public string debugcard = "gftc:3618bfa66c9140dda285ad592de66999";
    public string debugGAN = "7783320007717440";

    public string customername;
    public string customerID;
    public string customercard;
    public string cardGAN;
    public int cardBalance;
		public int startcardBalance;

    public string filename = "L1.txt";
		public string visitedname = "V1.txt";
    public string path;
    public string visitedpath;

    List<string> userInfo = new List<string>();
    List<string> visitedList = new List<string>();

		public bool pluscodes;
    public bool _cleanStart;

    void Start()
    {
      path = BaseDir() + Separator() + filename;
      visitedpath = BaseDir() + Separator() + visitedname;

      if(_cleanStart == true)
      {
        if(File.Exists(path))
        {
          File.Delete(path);
        }
				if(File.Exists(visitedpath))
        {
          File.Delete(visitedpath);
        }
      }
      else
      {

      }

      StartCoroutine(setUpLoop());
    }

	public void wipeData()
	{
		if(File.Exists(path))
    {
      File.Delete(path);
    }
		if(File.Exists(visitedpath))
    {
      File.Delete(visitedpath);
    }
	}

    void Update()
    {
      if(Input.GetKeyUp(KeyCode.M))
      {
        rewardObject.SetActive(true);
      }
      if(Input.GetKeyUp(KeyCode.N))
      {
				rewardObject.SetActive(false);
      }
      if(Input.GetKeyUp(KeyCode.B))
      {
        StartCoroutine(CreateGiftCard());
      }
      if(Input.GetKeyUp(KeyCode.V))
      {
        StartCoroutine(ActivateGiftCard("7783320001143692", "gftc:373e3fb6c7954afd8c4822f9a09ff156"));
      }
      if(Input.GetKeyUp(KeyCode.C))
      {
        StartCoroutine(GetGiftCard(debugcard));
      }
      if(Input.GetKeyUp(KeyCode.X))
      {
        StartCoroutine(setUpLoop());
      }
      if(Input.GetKeyUp(KeyCode.Z))
      {
        StartCoroutine(LoadUpGiftCard(10));
      }
    }

	IEnumerator setUpVisited()
	{
		Debug.Log("starting set up previously visited pluscodes");

		if(File.Exists(visitedpath))
        {
            string str = ReadFileContent(visitedpath);
						visitedList = str.Split(":").ToList();
						foreach(string s in visitedList)
						{
							if(s.Contains(","))
							{
								kml.visitedpluscodeList.Add(s);
							}
						}
        }
		yield return new WaitForEndOfFrame();

		Debug.Log("setUpVisited complete");
	}

    IEnumerator setUpLoop()
    {
			Debug.Log("starting Square set up");
		loadingObject.SetActive(true);

#if !UNITY_EDITOR
		while(mapObject.activeSelf == false)
		{
			yield return new WaitForEndOfFrame();
		}
#endif

		if(pluscodes == true)
		{
			yield return setUpVisited();
		}

      if(File.Exists(path))
      {
				Debug.Log("save file found");

        string str = ReadFileContent(path);
        userInfo = str.Split(",").ToList();
        customername = userInfo[0];
        customerID = userInfo[1];
        customercard =  userInfo[2];
        cardGAN = userInfo[3];
        int.TryParse(userInfo[4], out cardBalance);
				startcardBalance = cardBalance;
      }
      else
      {
				Debug.Log("no save file found, create new customer");
        customername = GenerateRandomStringBasedOnDateTime();
        yield return CreateCustomer(customername);
      }

      yield return RetrieveCustomer(customerID);

      yield return new WaitForEndOfFrame();

      WriteToFile(customername + "," + customerID + "," + customercard + "," + cardGAN + "," + cardBalance.ToString(), path);

	  	geo.squareReady = true;

	  	loadingObject.SetActive(false);

			rewardTextmain.text = "¥" + cardBalance.ToString();

			usernameText.text = "Username: " + customername;
			userIDText.text = "User ID: " + customerID;
			userCardText.text = "Gift Card ID: " + customercard;
			userCardGanText.text = "Gift Card GAN: " + cardGAN;
			userBalanceText.text = "Gift Card Balance: ¥" + cardBalance;
	}

    IEnumerator CreateCustomer(string givenName)
    {
        CreateCustomerRequest requestData = new CreateCustomerRequest
        {
            idempotency_key = Guid.NewGuid().ToString(),
            given_name = givenName
        };

        string bodyJsonString = JsonUtility.ToJson(requestData);
        yield return StartCoroutine(PostRequest(baseUrl + customerEndpoint, bodyJsonString, 0));
    }

    IEnumerator RetrieveCustomer(string customerId)
    {
        yield return StartCoroutine(GetRequest(baseUrl + customerEndpoint + "/" + customerId));
    }

    IEnumerator CreateGiftCard()
    {
        GiftCard giftCard = new GiftCard
        {
            type = "DIGITAL"
        };

        CreateGiftCardRequest requestData = new CreateGiftCardRequest
        {
            idempotency_key = Guid.NewGuid().ToString(),
            location_id = "LWW58KBXJSKY1",
            gift_card = giftCard
        };

        string bodyJsonString = JsonUtility.ToJson(requestData);
        yield return StartCoroutine(PostRequest(baseUrl + giftCardEndpoint, bodyJsonString, 1));
      }

      IEnumerator LinkCustomerToGiftCard(string customerId, string giftCardId)
      {
          LinkCustomerToGiftCardRequest requestData = new LinkCustomerToGiftCardRequest
          {
              customer_id = customerId
          };

          string bodyJsonString = JsonUtility.ToJson(requestData);
          yield return StartCoroutine(PostRequest(baseUrl + linkGiftCardEndpoint + giftCardId + "/link-customer", bodyJsonString, 2));
      }


      public IEnumerator ActivateGiftCard(string gan, string giftCardId)
      {
          GiftCardActivityRequestData requestData = new GiftCardActivityRequestData
          {
              gift_card_activity = new GiftCardActivityForActivation
              {
                  location_id = "LWW58KBXJSKY1",
                  type = "ACTIVATE",
                  gift_card_gan = gan,
                  gift_card_id = giftCardId,
                  activate_activity_details = new ActivateActivityDetails
                  {
                      buyer_payment_instrument_ids = new List<string> { GenerateRandomStringBasedOnDateTime() },  // Adjust as needed
                      amount_money = new Money
                      {
                          amount = 1,
                          currency = "GBP"
                      }
                  }
              },
              idempotency_key = Guid.NewGuid().ToString()
          };

          string jsonBody = JsonUtility.ToJson(requestData);

          Debug.Log(jsonBody);

          UnityWebRequest request = new UnityWebRequest(baseUrl + createGiftCardActivityEndpoint, "POST");
          byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
          request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
          request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
          request.SetRequestHeader("Content-Type", "application/json");
          request.SetRequestHeader("Authorization", "Bearer " + accessToken);
          request.SetRequestHeader("Square-Version", "2023-08-16");

          yield return request.SendWebRequest();

          if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
          {
              Debug.LogError(request.error);
          }
          else
          {
              Debug.Log(request.downloadHandler.text);  // Log the server's response
          }
      }

      IEnumerator PostRequest(string url, string bodyJsonString, int stage)
      {
          Debug.Log(url); //

          var request = new UnityWebRequest(url, "POST");
          byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJsonString);
          request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
          request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
          request.SetRequestHeader("Content-Type", "application/json");
          request.SetRequestHeader("Authorization", "Bearer " + accessToken);

          yield return request.SendWebRequest();

          if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
          {
              Debug.LogError("Error: " + request.error);
          }
          else if (request.responseCode >= 400)
          {
              ApiResponseError apiResponse = JsonUtility.FromJson<ApiResponseError>(request.downloadHandler.text);
              foreach (var error in apiResponse.errors)
              {
                  Debug.LogError($"Category: {error.category}, Code: {error.code}, Detail: {error.detail}");
              }
          }
          else
          {
              Debug.Log("Received: " + request.downloadHandler.text); //
              if(stage == 0)
              {
                Root rootObject = JsonUtility.FromJson<Root>(request.downloadHandler.text);
                customerID = rootObject.customer.id;
              }
              if(stage == 1)
              {
                //Root rootObject = JsonUtility.FromJson<Root>(request.downloadHandler.text);
                //customerID = rootObject.customer.id;
                GiftCardResponse response = JsonUtility.FromJson<GiftCardResponse>(request.downloadHandler.text);
                customercard = response.gift_card.id;
                cardGAN = response.gift_card.gan;
                cardBalance = response.gift_card.balance_money.amount;
                string balanceCurrency = response.gift_card.balance_money.currency;

                Debug.Log($"Gift Card ID: {customercard}");
                Debug.Log($"Balance: {cardBalance} {balanceCurrency}");

								rewardTextmain.text = "¥" + cardBalance.ToString();
								userBalanceText.text = "Gift Card Balance: ¥" + cardBalance;
              }
              if(stage == 2)
              {

              }
              if(stage == 3)
              {

              }
              if(stage == 4)
              {

              }
          }
      }

      IEnumerator GetRequest(string url)
      {
					Debug.Log("try RetrieveCustomer");
          using (UnityWebRequest request = UnityWebRequest.Get(url))
          {
              request.SetRequestHeader("Square-Version", "2023-08-16");
              request.SetRequestHeader("Authorization", "Bearer " + accessToken);
              request.SetRequestHeader("Content-Type", "application/json");

              yield return request.SendWebRequest();

              if (request.result != UnityWebRequest.Result.Success)
              {
                  Debug.LogError($"Failed to make the request. Response Code: {request.responseCode}, Exception: {request.error}");
              }
              else
              {
                  Debug.Log("Customer Retrieved: " + request.downloadHandler.text);
                  CustomerResponse response = JsonUtility.FromJson<CustomerResponse>(request.downloadHandler.text);

                   if (response.customer.cards != null && response.customer.cards.Length > 0)
                   {
                       Debug.Log($"User has a card. Card ID: {response.customer.cards[0].id}");
                       customercard = response.customer.cards[0].id;
                       yield return StartCoroutine(GetGiftCard(customercard));
                   }
                   else
                   {
                       Debug.Log("User does not have a card.");
                       yield return StartCoroutine(CreateGiftCard());
                       yield return new WaitForEndOfFrame();
                       yield return StartCoroutine(LinkCustomerToGiftCard(customerID, customercard));
                       yield return new WaitForEndOfFrame();
                       yield return StartCoroutine(GetGiftCard(customercard));
                       yield return new WaitForEndOfFrame();
                       yield return StartCoroutine(ActivateGiftCard(cardGAN, customercard));
                   }
              }
          }
      }

      IEnumerator GetGiftCard(string id)
      {
        yield return StartCoroutine(GetGiftCardData(baseUrl + "/v2/gift-cards/" + id));
      }

      IEnumerator GetGiftCardData(string URL)
      {
          using (UnityWebRequest www = UnityWebRequest.Get(URL))
          {
              www.SetRequestHeader("Square-Version", "2023-08-16");
              www.SetRequestHeader("Authorization", "Bearer " + accessToken);
              www.SetRequestHeader("Content-Type", "application/json");

              yield return www.SendWebRequest();

              if (www.result != UnityWebRequest.Result.Success)
              {
                  Debug.Log(www.error);
              }
              else
              {
                  string responseBody = www.downloadHandler.text;
                  GiftCardDataResponse response = JsonUtility.FromJson<GiftCardDataResponse>(responseBody);
                  Debug.Log("Gift Card ID: " + response.gift_card.id);
                  Debug.Log("Gift Card Balance: " + response.gift_card.balance_money.amount + " " + response.gift_card.balance_money.currency);
                  Debug.Log("Gift Card GAN: " + response.gift_card.gan);

                  cardBalance = response.gift_card.balance_money.amount;
                  cardGAN = response.gift_card.gan;
									startcardBalance = cardBalance;

									rewardTextmain.text = "¥" + cardBalance.ToString();
									userBalanceText.text = "Gift Card Balance: ¥" + cardBalance;

                  if(response.gift_card.customer_ids != null && response.gift_card.customer_ids.Length > 0)
                  {
                      Debug.Log("Customer ID: " + response.gift_card.customer_ids[0]);
                  }
                  else
                  {
                      Debug.Log("No customer ID linked with this gift card.");
                  }
              }
          }
      }

	public void addBonus(int amount)
	{
		StartCoroutine(LoadUpGiftCard(amount));

		if(pluscodes == true)
		{
			string visited = "";
			foreach(string s in kml.visitedpluscodeList)
			{
				visited += s + ":";
			}
			WriteToFile(visited, visitedpath);
		}
	}

    IEnumerator LoadUpGiftCard(int amount)
    {
      yield return StartCoroutine(PostGiftCardActivityLoad(baseUrl + createGiftCardActivityEndpoint, customercard, cardGAN, amount));
    }

    private IEnumerator PostGiftCardActivityLoad(string URL, string id, string gan, int bonus)
    {
        PostRequestBodyLoad body = new PostRequestBodyLoad
        {
            gift_card_activity = new LoadGiftCardActivity
            {
                location_id = "LWW58KBXJSKY1",
                type = "LOAD",
                gift_card_gan = gan,
                load_activity_details = new LoadActivityDetails
                {
                    amount_money = new Money { currency = "GBP", amount = bonus },
                    buyer_payment_instrument_ids = new string[] { GenerateRandomStringBasedOnDateTime() }
                },
                gift_card_id = id
            },
            idempotency_key = Guid.NewGuid().ToString()
        };

        string json = JsonUtility.ToJson(body);

        using (UnityWebRequest www = new UnityWebRequest(URL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Square-Version", "2023-08-16");
            www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                UniqueGiftCardActivityResponse parsedResponse = JsonUtility.FromJson<UniqueGiftCardActivityResponse>(www.downloadHandler.text);
                cardBalance = parsedResponse.gift_card_activity.gift_card_balance_money.amount;
                string currency = parsedResponse.gift_card_activity.gift_card_balance_money.currency;

                Debug.Log($"Card Balance: {cardBalance} {currency}");
                WriteToFile(customername + "," + customerID + "," + customercard + "," + cardGAN + "," + cardBalance.ToString(), path);
								StartCoroutine(fadeOutText(cardBalance.ToString()));
								rewardTextmain.text = "¥" + cardBalance.ToString();

								int totalGain = cardBalance - startcardBalance;
								rewardTextearnings.text = "¥" + totalGain.ToString();
								userBalanceText.text = "Gift Card Balance: ¥" + cardBalance;

						}
        }
    }

    [System.Serializable]
    public class ApiResponseError
    {
        public Error[] errors;
    }

    [System.Serializable]
    public class Error
    {
        public string category;
        public string code;
        public string detail;
    }

    [System.Serializable]
    private class CreateCustomerRequest
    {
        public string idempotency_key;
        public string given_name;
        // ... other fields if needed ...
    }

    [System.Serializable]
    public class GiftCard
    {
        public string type;
    }

    [System.Serializable]
    public class CreateGiftCardRequest
    {
        public string idempotency_key;
        public string location_id;
        public GiftCard gift_card;
    }

    [System.Serializable]
    public class LinkCustomerToGiftCardRequest
    {
        public string customer_id;
    }

    [System.Serializable]
    public class Money
    {
        public long amount;
        public string currency;
    }

    [System.Serializable]
    public class GiftCardActivityActivate
    {
        public Money amount_money;
        public List<string> buyer_payment_instrument_ids;
    }

    [System.Serializable]
    public class GiftCardActivity
    {
        public string type;
        public string location_id;
        public string gift_card_gan;
        public GiftCardActivityActivate activate_activity_details;
        public GiftCardActivityLoad load_activity_details;
    }

    [System.Serializable]
    public class CreateGiftCardActivityRequest
    {
        public string idempotency_key;
        public GiftCardActivity gift_card_activity;
    }

    [System.Serializable]
    public class GiftCardActivityLoad
    {
        public Money amount_money;
        public List<string> buyer_payment_instrument_ids;
    }

    [Serializable]
    public class Customer
    {
        public string id;
        public string created_at;
        public string updated_at;
        public string given_name;
        public Preferences preferences;
        public string creation_source;
        public int version;
    }

    [Serializable]
    public class Preferences
    {
        public bool email_unsubscribed;
    }

    [Serializable]
    public class Root
    {
        public Customer customer;
    }

    [System.Serializable]
    public class Card
    {
        public string id;
        public string card_brand;
        public string last_4;
        public int exp_month;
        public int exp_year;
        // ... other fields if needed
    }

    [System.Serializable]
    public class CustomerData
    {
        public string id;
        public Card[] cards;
        // ... other fields if needed
    }

    [System.Serializable]
    public class CustomerResponse
    {
        public CustomerData customer;
    }

    [System.Serializable]
    public class BalanceMoney
    {
        public int amount;
        public string currency;
    }

    [System.Serializable]
    public class GiftCardRetrieved
    {
        public string id;
        public string gan;
        public BalanceMoney balance_money;
        // ... other fields if needed
    }

    [System.Serializable]
    public class GiftCardResponse
    {
        public GiftCardRetrieved gift_card;
    }

    [System.Serializable]
    public class GiftCardData
    {
        public string id;
        public string type;
        public string gan_source;
        public string state;
        public BalanceMoney balance_money;
        public string gan;
        public string created_at;
        public string[] customer_ids;
    }

    [System.Serializable]
    public class GiftCardDataResponse
    {
        public GiftCardData gift_card;
    }

    [System.Serializable]
    public class LoadActivityDetails
    {
        public Money amount_money;
        public string[] buyer_payment_instrument_ids;
    }

    [System.Serializable]
    public class LoadGiftCardActivity
    {
        public string location_id;
        public string type;
        public string gift_card_gan;
        public LoadActivityDetails load_activity_details;
        public string gift_card_id;
    }

    [System.Serializable]
    public class PostRequestBodyLoad
    {
        public LoadGiftCardActivity gift_card_activity;
        public string idempotency_key;
    }

    [System.Serializable]
    public class UniqueBalanceMoney
    {
        public int amount;
        public string currency;
    }

    [System.Serializable]
    public class UniqueLoadActivityDetails
    {
        public UniqueBalanceMoney amount_money;
        public List<string> buyer_payment_instrument_ids;
    }

    [System.Serializable]
    public class UniqueGiftCardActivityData
    {
        public string id;
        public string type;
        public string location_id;
        public string created_at;
        public string gift_card_id;
        public string gift_card_gan;
        public UniqueBalanceMoney gift_card_balance_money;
        public UniqueLoadActivityDetails load_activity_details;
    }

    [System.Serializable]
    public class UniqueGiftCardActivityResponse
    {
        public UniqueGiftCardActivityData gift_card_activity;
    }

    [System.Serializable]
    public class ActivateActivityDetails
    {
        public List<string> buyer_payment_instrument_ids;
        public Money amount_money;
    }

    [System.Serializable]
    public class GiftCardActivityForActivation
    {
        public string location_id;
        public string type;
        public string gift_card_gan;
        public string gift_card_id;
        public ActivateActivityDetails activate_activity_details;
    }

    [System.Serializable]
    public class GiftCardActivityRequestData
    {
        public GiftCardActivityForActivation gift_card_activity;
        public string idempotency_key;
    }

    char Separator()
    {
        return System.IO.Path.DirectorySeparatorChar;
    }

    string BaseDir()
    {
      var base_dir = Application.persistentDataPath ;

      #if UNITY_EDITOR
         base_dir = Application.streamingAssetsPath;
      #endif

        return base_dir;
    }

    string ReadFileContent(string thepath)
    {
        string content = "";
        if(File.Exists(thepath))
        {
          FileInfo fi = new FileInfo(thepath);
          try
          {
              using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
              {
                  content = sr.ReadToEnd();
              }
          }
          catch (Exception e)
          {
              Debug.LogError(e);
              content = "";
          }
        }
        return content;
    }

    public void WriteToFile(string _strings, string thepath)
    {
        string _s = thepath;
        string _string = _strings;
        StreamWriter sw = new StreamWriter(_s);
        sw.WriteLine(_string);
        sw.Close();
    }

    string GenerateRandomStringBasedOnDateTime()
    {
        string dateTimeString = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // This will create a string representation of the current date and time down to milliseconds
        string randomAlphaNumeric = GenerateRandomAlphaNumeric(5); // Generate a random 5-character alphanumeric string
				Debug.Log("new customer name generated: " + dateTimeString + randomAlphaNumeric);
	      return dateTimeString + randomAlphaNumeric;
    }

    string GenerateRandomAlphaNumeric(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

		IEnumerator fadeOutText(string s)
		{
			rewardTextmain.text = "¥" + s;
			var spin = textColour;
			var coin = rewardColour;
			float alph = 0;
			spin.a = alph;
			coin.a = alph;
			rewardText.color = spin;
			rewardTextTwo.color = spin;
			rewardImage.color = coin;
			yield return new WaitForEndOfFrame();
			rewardObject.SetActive(true);
			while(alph < 1)
			{
				alph = spin.a;
				alph += 0.03f;
				spin.a = alph;
				coin.a = alph;
				rewardText.color = spin;
				rewardTextTwo.color = spin;
				rewardImage.color = coin;
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
			while(alph > 0)
			{
				alph = spin.a;
				alph -= 0.006f;
				spin.a = alph;
				coin.a = alph;
				rewardText.color = spin;
				rewardTextTwo.color = spin;
				rewardImage.color = coin;
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
			rewardObject.SetActive(false);
		}

		bool infoout;

		public void triggerUserInfo()
		{
			if(infoout == false)
			{
				StartCoroutine(showUserInfo());
			}
		}

		IEnumerator showUserInfo()
		{
			infoout = true;
			userInfoAnimator.Play("showinfo",0,0);
			yield return new WaitForSeconds(4);
			infoout = false;
		}

		public void resetData()
		{
			StartCoroutine(resetLoop());
		}

		IEnumerator resetLoop()
		{
			if(geo != null)
			{
				geo.stopChecking();
			}

			if(File.Exists(path))
			{
				File.Delete(path);
			}
			if(File.Exists(visitedpath))
			{
				File.Delete(visitedpath);
			}

			kml.nameList.Clear();
			kml.latLonList.Clear();
			kml.centerList.Clear();
			kml.pluscodeList.Clear();
			kml.visitedpluscodeList.Clear();
			kml.anchorpluscodeList.Clear();
			rewardTextearnings.text = "¥0";

			if(geo != null)
			{
				geo.OnClearAllClicked();
				customgeo.clearAnchors();
			}

			yield return new WaitForSeconds(1f);

			StartCoroutine(setUpLoop());

			if(geo != null)
			{
				geo.startChecking();
			}
		}
}
