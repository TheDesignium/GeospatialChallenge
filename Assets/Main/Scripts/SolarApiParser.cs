using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class SolarApiParser : MonoBehaviour
{

    public GeospatialController geo;

    public TMP_Text debugTxt;

    public TMP_Text numberTxt;
    public TMP_Text wattsTxt;
    public TMP_Text areaTxt;
    public TMP_Text areaTxt2;
    public TMP_Text sunshineTxt;
    public TMP_Text qualityTxt;

    public TMP_Text countTxt;
    public TMP_Text sunTxtA;
    public TMP_Text sunTxtB;

    public TMP_Text datetext;

    public Animator ani;
    public bool hidden = true;

	  public Transform targetpoint;
    public Transform fakeParent;

    public GameObject loading;
    public GameObject tap;
	  public GameObject visuals;
	  public GameObject sun;
    public GameObject objectPrefab; // Prefab of the object you want to place
	  public List<GameObject> panels = new List<GameObject>();

    List<CompleteBoundingBox> bboxList = new List<CompleteBoundingBox>();

	public Transform[] areaCorners;

	public Vector3 targetPositionA = new Vector3();
	public Vector3 targetPositionB = new Vector3();
	public Vector3 targetPositionC = new Vector3();
  public Vector3 targetPositionD = new Vector3();

	public float lerpSpeed;

    public Transform parentTransform; // Parent transform to group the objects
    public int numberOfObjects = 9; // Number of objects to place
    public float areaInSquareMeters = 100.0f; // Area of the square in square meters

	  public bool setUp;

    void Start()
    {
      hidden = true;
    }

    public void checkData(string json)
    {
        countTxt.text = "Potential Solar Panels: 0";

        BuildingSolarData data = JsonUtility.FromJson<BuildingSolarData>(json);
        // Now you can access the parsed data using the 'data' object.
        Debug.Log(data.name);

        //google.OnClearMapClick();
        bboxList.Clear();

        if (data != null && data.solarPotential != null && data.solarPotential.maxArrayPanelsCount > 0)
        {
            Debug.Log($"Max Array Panels Count: {data.solarPotential.maxArrayPanelsCount}");
            debugTxt.text = data.solarPotential.maxArrayPanelsCount.ToString();

            string numberpanels = data.solarPotential.maxArrayPanelsCount.ToString();
            string watts = data.solarPotential.panelCapacityWatts.ToString();
            string area = data.solarPotential.maxArrayAreaMeters2.ToString("F0");
            string sunshine = data.solarPotential.maxSunshineHoursPerYear.ToString("F0");
            float count = data.solarPotential.wholeRoofStats.sunshineQuantiles.Count();
            float sum = data.solarPotential.wholeRoofStats.sunshineQuantiles.Sum();
            float qual = sum/count;
            string quality = qual.ToString("F0");

            Debug.Log(data.solarPotential.maxArrayAreaMeters2 + "/" + data.solarPotential.wholeRoofStats.groundAreaMeters2 + "=" + data.solarPotential.maxArrayAreaMeters2/data.solarPotential.wholeRoofStats.groundAreaMeters2);
            Debug.Log(data.solarPotential.maxSunshineHoursPerYear/4380f);

            if(json.Contains("financialAnalyses"))
            {
              CashPurchaseSavings cashPurchaseSavings = data.financialAnalyses[0].cashPurchaseSavings; // Assuming you want the first financial analysis

              // Log the properties of the CashPurchaseSavings object
              Debug.Log("Financially Viable: " + cashPurchaseSavings.savings.financiallyViable);
              Debug.Log("Out of Pocket Cost: " + cashPurchaseSavings.outOfPocketCost.currencyCode + " " + cashPurchaseSavings.outOfPocketCost.units);
              Debug.Log("Upfront Cost: " + cashPurchaseSavings.upfrontCost.currencyCode + " " + cashPurchaseSavings.upfrontCost.units);
              Debug.Log("Payback Years: " + cashPurchaseSavings.paybackYears);
              Debug.Log("Savings Year 1: " + cashPurchaseSavings.savings.savingsYear1.currencyCode + " " + cashPurchaseSavings.savings.savingsYear1.units);
              Debug.Log("Savings Year 20: " + cashPurchaseSavings.savings.savingsYear20.currencyCode + " " + cashPurchaseSavings.savings.savingsYear20.units);
            }

            numberTxt.text = "Maximum  number of Solar Panels: " + numberpanels;
            wattsTxt.text = "Maximum  capacity in watts: " + watts;
            areaTxt.text = "Available area: " + area + " m2";
            areaTxt2.text = "Available area: " + area + " m2";
            sunshineTxt.text = "Sunshine hours per year: " + sunshine;
            qualityTxt.text = "Average sunshine quality: " + quality;

			      sunTxtA.text = "Sunshine hours per year: " + sunshine;
            sunTxtB.text = "Average sunshine quality: " + quality;

            ani.Play("show",0,0);
            hidden = false;
            loading.SetActive(false);

            string a = data.solarPotential.panelHeightMeters.ToString();
            string b = data.solarPotential.panelWidthMeters.ToString();

			//imageryQuality
			     datetext.text = "Data from " + data.imageryDate.year + ". Quality: " + data.imageryQuality;
			//imageryDate

            //Debug.Log(a + ":" + b);

            foreach (var segment in data.solarPotential.roofSegmentStats)
            {
                //Debug.Log("Pitch Degrees: " + segment.pitchDegrees);
                //BoundingBox box = segment.boundingBox;
                //CompleteBoundingBox completeBox = GetCompleteBoundingBox(box);
                //bboxList.Add(completeBox);
                //google.setAreaPlusCodes(completeBox.NE.x,completeBox.NE.y, completeBox.SW.x,completeBox.SW.y);
            }

      			areaInSquareMeters = data.solarPotential.maxArrayAreaMeters2;
      			numberOfObjects = data.solarPotential.maxArrayPanelsCount;

      			float sideLength = Mathf.Sqrt(data.solarPotential.maxArrayAreaMeters2);
            float halfLength = sideLength/2;

      			targetPositionA = new Vector3(-halfLength,0,sideLength);
      			targetPositionB = new Vector3(-halfLength,0,0);
      			targetPositionC = new Vector3(halfLength,0,0);
            targetPositionD = new Vector3(halfLength,0,sideLength);

      			StartCoroutine(setUpLoop(sideLength * 0.9f));

      			setUp = true;
        }
        else
        {
            Debug.LogWarning("Data not parsed or incomplete!");
            geo.setTap();
        }
    }

	void Update()
	{
		/*
		if(setUp == true)
		{
			areaCorners[1].position = Vector3.Lerp(areaCorners[1].position, targetPositionA, lerpSpeed * Time.deltaTime);
			areaCorners[2].position = Vector3.Lerp(areaCorners[2].position, targetPositionB, lerpSpeed * Time.deltaTime);
			areaCorners[3].position = Vector3.Lerp(areaCorners[3].position, targetPositionC, lerpSpeed * Time.deltaTime);
		}
		*/
	}

	IEnumerator setUpLoop(float sideLength)
	{
		visuals.SetActive(false);
    areaCorners[1].localPosition = new Vector3(0, 0, 0);
    areaCorners[2].localPosition = new Vector3(0, 0, 0);
    areaCorners[3].localPosition = new Vector3(0, 0, 0);
    areaCorners[0].localPosition = new Vector3(0, 0, 0);

    countTxt.text = "Potential Solar Panels: 0";

    visuals.transform.position = targetpoint.position;
	  Vector3 v3Position = visuals.transform.position;
    v3Position.y = Camera.main.transform.position.y;
    visuals.transform.position = v3Position;

		Vector3 v3 = new Vector3(Camera.main.transform.position.x, visuals.transform.position.y, Camera.main.transform.position.z);
		visuals.transform.LookAt(v3);
		foreach(GameObject g in panels)
		{
			Destroy(g);
		}
		panels.Clear();

		yield return new WaitForEndOfFrame();
		visuals.SetActive(true);

		ani.Play("show",0,0);

		float distance = 9999f;
		while(distance > 0.04f)
		{
			areaCorners[1].localPosition = Vector3.Lerp(areaCorners[1].localPosition, targetPositionA, lerpSpeed * Time.deltaTime);
			areaCorners[2].localPosition = Vector3.Lerp(areaCorners[2].localPosition, targetPositionB, lerpSpeed * Time.deltaTime);
			areaCorners[3].localPosition = Vector3.Lerp(areaCorners[3].localPosition, targetPositionC, lerpSpeed * Time.deltaTime);
      areaCorners[0].localPosition = Vector3.Lerp(areaCorners[0].localPosition, targetPositionD, lerpSpeed * Time.deltaTime);

			distance = Vector3.Distance(areaCorners[3].localPosition, targetPositionC);
			yield return new WaitForEndOfFrame();
		}

    distance = Vector3.Distance(areaCorners[0].position, areaCorners[1].position);

    Debug.Log(sideLength + "::" + distance);

		// Calculate the number of rows and columns
		int numRows = Mathf.CeilToInt(Mathf.Sqrt(numberOfObjects));
		int numCols = Mathf.CeilToInt((float)numberOfObjects / numRows);

    yield return new WaitForSeconds(0.5f);

		// Calculate the spacing between objects
		float horizontalSpacing = sideLength / numCols;
		float verticalSpacing = sideLength / numRows;

		//Debug.Log(numRows + ":" + numCols);
		//Debug.Log(horizontalSpacing + ":" + verticalSpacing);

		int counter = 0;

		for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                // Calculate the position for the current object
                float xPos = col * horizontalSpacing - sideLength / 2 + horizontalSpacing / 2;
                float yPos = row * verticalSpacing - sideLength / 2 + verticalSpacing / 2;

        				if(counter < numberOfObjects)
        				{
        					GameObject newObj = Instantiate(objectPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);

        					// Set the parent to group the objects
        					newObj.transform.parent = fakeParent;
        					newObj.transform.localPosition = new Vector3(xPos, 0, yPos);
        					newObj.transform.localEulerAngles = new Vector3(0, -90, 0);
        					newObj.transform.localScale = new Vector3(horizontalSpacing * 0.9f, horizontalSpacing * 0.9f, horizontalSpacing * 0.9f);
        					panels.Add(newObj);

        					counter += 1;

        					countTxt.text = "Potential Solar Panels: " + counter.ToString();

        					//yield return new WaitForSeconds(0.05f);
                  yield return new WaitForEndOfFrame();
        				}

            }
        }

		sun.SetActive(true);
		sun.transform.parent = null;
	}

    public void hideUI()
    {
      loading.SetActive(true);
      tap.SetActive(false);
      if(hidden == false)
      {
        hidden = true;
        ani.Play("hide",0,0);
      }
    }

    private CompleteBoundingBox GetCompleteBoundingBox(BoundingBox box)
    {
        // NE and SW are given
        Vector2 NE = new Vector2(box.ne.latitude, box.ne.longitude);
        Vector2 SW = new Vector2(box.sw.latitude, box.sw.longitude);

        // Compute NW and SE based on NE and SW
        Vector2 NW = new Vector2(NE.x, SW.y);
        Vector2 SE = new Vector2(SW.x, NE.y);

        return new CompleteBoundingBox(NE, NW, SE, SW);
    }
}

[System.Serializable]
public class BuildingSolarData
{
    public string name;
    public Center center;
    public ImageryDate imageryDate;
    public string postalCode;
    public string administrativeArea;
    public string statisticalArea;
    public string regionCode;
    public SolarPotential solarPotential;

    //public FinancialAnalysis[] financialAnalyses;
    public float panelCapacityWatts;
    public float panelHeightMeters;
    public float panelWidthMeters;
    public int panelLifetimeYears;
    /*
    public BuildingStats buildingStats;
    public SolarPanel[] solarPanels;
    public string imageryQuality;
    public ImageryDate imageryProcessedDate;
    */
    public string imageryQuality;
    public FinancialAnalysis[] financialAnalyses; // Add this line

}

[System.Serializable]
public class Center
{
    public float latitude;
    public float longitude;
}

[System.Serializable]
public class ImageryDate
{
    public int year;
    public int month;
    public int day;
}

[System.Serializable]
public class SolarPotential
{
    public int maxArrayPanelsCount;
    public float panelCapacityWatts;
    public float panelHeightMeters;
    public float panelWidthMeters;
    public float maxArrayAreaMeters2;
    public float maxSunshineHoursPerYear;
    public float carbonOffsetFactorKgPerMwh;
    public WholeRoofStats wholeRoofStats;
    public RoofSegmentStat[] roofSegmentStats;

    //public SolarPanelConfig[] solarPanelConfigs;
}

// ... More classes following the same pattern ...

// Example class for WholeRoofStats:
[System.Serializable]
public class WholeRoofStats
{
    public float areaMeters2;
    public float[] sunshineQuantiles;
    public float groundAreaMeters2;
}

[System.Serializable]
public class RoofSegmentStat
{
    public float pitchDegrees;
    public float azimuthDegrees;
    public Stat stats;
    public Center center;
    public BoundingBox boundingBox;
    public float planeHeightAtCenterMeters;
    // ... Any other fields you need.
}

[System.Serializable]
public class Stat
{
    public float areaMeters2;
    public float[] sunshineQuantiles;
    public float groundAreaMeters2;
    // ... Any other fields you need.
}

[System.Serializable]
public class BoundingBox
{
    public SW sw;
    public NE ne;
}

[System.Serializable]
public class SW
{
    public float latitude;
    public float longitude;
}

[System.Serializable]
public class NE
{
    public float latitude;
    public float longitude;
}

public class CompleteBoundingBox
{
    public Vector2 NE;
    public Vector2 NW;
    public Vector2 SE;
    public Vector2 SW;

    public CompleteBoundingBox(Vector2 NE, Vector2 NW, Vector2 SE, Vector2 SW)
    {
        this.NE = NE;
        this.NW = NW;
        this.SE = SE;
        this.SW = SW;
    }
}

[System.Serializable]
public class FinancialAnalysis
{
    public MonthlyBill monthlyBill;
    public int panelConfigIndex;
    public FinancialDetails financialDetails;
    public LeasingSavings leasingSavings;
    public CashPurchaseSavings cashPurchaseSavings;
    public FinancedPurchaseSavings financedPurchaseSavings;
}

[System.Serializable]
public class MonthlyBill
{
    public string currencyCode;
    public string units;
}

[System.Serializable]
public class FinancialDetails
{
    public float initialAcKwhPerYear;
    public MonthlyBill remainingLifetimeUtilityBill;
    public MonthlyBill federalIncentive;
    public MonthlyBill stateIncentive;
    public MonthlyBill utilityIncentive;
    public MonthlyBill lifetimeSrecTotal;
    public MonthlyBill costOfElectricityWithoutSolar;
    public bool netMeteringAllowed;
    public float solarPercentage;
    public float percentageExportedToGrid;
}

[System.Serializable]
public class LeasingSavings
{
    public bool leasesAllowed;
    public bool leasesSupported;
    public MonthlyBill annualLeasingCost;
    public Savings savings;
}

[System.Serializable]
public class CashPurchaseSavings
{
    public MonthlyBill outOfPocketCost;
    public MonthlyBill upfrontCost;
    public MonthlyBill rebateValue;
    public float paybackYears;
    public Savings savings;
}

[System.Serializable]
public class FinancedPurchaseSavings
{
    public MonthlyBill annualLoanPayment;
    public MonthlyBill rebateValue;
    public float loanInterestRate;
    public Savings savings;
}

[System.Serializable]
public class Savings
{
    public MonthlyBill savingsYear1;
    public MonthlyBill savingsYear20;
    public MonthlyBill presentValueOfSavingsYear20;
    public bool financiallyViable;
    public MonthlyBill savingsLifetime;
    public MonthlyBill presentValueOfSavingsLifetime;
}

/*
{
  "name": "buildings/ChIJo0bkheSKGGARlDk2RqF-QjM",
  "center": {
    "latitude": 35.6254844,
    "longitude": 139.7195552
  },
  "imageryDate": {
    "year": 2022,
    "month": 4,
    "day": 22
  },
  "regionCode": "JP",
  "solarPotential": {
    "maxArrayPanelsCount": 82,
    "maxArrayAreaMeters2": 134.21759,
    "maxSunshineHoursPerYear": 1223.011,
    "carbonOffsetFactorKgPerMwh": 550.9993,
    "wholeRoofStats": {
      "areaMeters2": 269.8619,
      "sunshineQuantiles": [
        380.6819,
        542.8878,
        609.9332,
        662.9728,
        693.5468,
        714.408,
        734.8582,
        761.8253,
        808.67456,
        922.2401,
        1268.8527
      ],
      "groundAreaMeters2": 184.03
    },
    "roofSegmentStats": [
      {
        "pitchDegrees": 56.44311,
        "azimuthDegrees": 297.1977,
        "stats": {
          "areaMeters2": 71.911255,
          "sunshineQuantiles": [
            526.6407,
            672.7816,
            692.9231,
            705.27594,
            716.3924,
            727.26587,
            738.6929,
            752.07666,
            775.75006,
            817.9236,
            953.3037
          ],
          "groundAreaMeters2": 39.75
        },
        "center": {
          "latitude": 35.6254766,
          "longitude": 139.7195001
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254316,
            "longitude": 139.7194611
          },
          "ne": {
            "latitude": 35.625536,
            "longitude": 139.71953449999998
          }
        },
        "planeHeightAtCenterMeters": 20.823046
      },
      {
        "pitchDegrees": 1.9404444,
        "azimuthDegrees": 315.9278,
        "stats": {
          "areaMeters2": 21.562365,
          "sunshineQuantiles": [
            411.04092,
            519.6597,
            617.9622,
            723.978,
            809.2421,
            854.04535,
            891.3931,
            928.17456,
            964.7252,
            994.80054,
            1112.1172
          ],
          "groundAreaMeters2": 21.55
        },
        "center": {
          "latitude": 35.6254566,
          "longitude": 139.71957
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254214,
            "longitude": 139.719525
          },
          "ne": {
            "latitude": 35.6254784,
            "longitude": 139.71962299999998
          }
        },
        "planeHeightAtCenterMeters": 27.77478
      },
      {
        "pitchDegrees": 55.097626,
        "azimuthDegrees": 299.9083,
        "stats": {
          "areaMeters2": 27.001999,
          "sunshineQuantiles": [
            517.9678,
            640.14374,
            695.1039,
            718.40405,
            736.57477,
            749.0475,
            756.34033,
            767.51154,
            780.778,
            799.4692,
            887.74274
          ],
          "groundAreaMeters2": 15.45
        },
        "center": {
          "latitude": 35.625477,
          "longitude": 139.7195344
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254456,
            "longitude": 139.7195113
          },
          "ne": {
            "latitude": 35.6255029,
            "longitude": 139.7195586
          }
        },
        "planeHeightAtCenterMeters": 24.476398
      },
      {
        "pitchDegrees": 48.028633,
        "azimuthDegrees": 18.241486,
        "stats": {
          "areaMeters2": 21.083824,
          "sunshineQuantiles": [
            480.19742,
            554.4259,
            602.3527,
            649.3011,
            670.5733,
            681.6382,
            694.5315,
            707.9065,
            719.2967,
            734.653,
            784.705
          ],
          "groundAreaMeters2": 14.1
        },
        "center": {
          "latitude": 35.6254929,
          "longitude": 139.71959479999998
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.625476,
            "longitude": 139.7195664
          },
          "ne": {
            "latitude": 35.6255126,
            "longitude": 139.7196222
          }
        },
        "planeHeightAtCenterMeters": 23.40036
      },
      {
        "pitchDegrees": 46.543484,
        "azimuthDegrees": 24.549465,
        "stats": {
          "areaMeters2": 18.98797,
          "sunshineQuantiles": [
            380.6819,
            389.5046,
            415.64798,
            556.541,
            645.3342,
            680.9356,
            696.41833,
            710.56287,
            731.507,
            795.97845,
            901.6388
          ],
          "groundAreaMeters2": 13.06
        },
        "center": {
          "latitude": 35.6255282,
          "longitude": 139.7196075
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6255121,
            "longitude": 139.7195769
          },
          "ne": {
            "latitude": 35.6255465,
            "longitude": 139.7196679
          }
        },
        "planeHeightAtCenterMeters": 18.254766
      },
      {
        "pitchDegrees": 3.7012386,
        "azimuthDegrees": 343.11264,
        "stats": {
          "areaMeters2": 11.634267,
          "sunshineQuantiles": [
            381.48022,
            390.51575,
            392.72568,
            415.6262,
            609.7415,
            704.9161,
            779.4634,
            923.111,
            964.47675,
            987.7863,
            1033.2643
          ],
          "groundAreaMeters2": 11.61
        },
        "center": {
          "latitude": 35.6255482,
          "longitude": 139.7195365
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6255293,
            "longitude": 139.71949379999998
          },
          "ne": {
            "latitude": 35.6255619,
            "longitude": 139.7195904
          }
        },
        "planeHeightAtCenterMeters": 17.32901
      },
      {
        "pitchDegrees": 17.043802,
        "azimuthDegrees": 296.8249,
        "stats": {
          "areaMeters2": 9.612157,
          "sunshineQuantiles": [
            407.99286,
            462.29935,
            552.8479,
            618.1907,
            725.5013,
            813.05743,
            912.22314,
            940.0822,
            949.9955,
            959.6304,
            1010.55725
          ],
          "groundAreaMeters2": 9.19
        },
        "center": {
          "latitude": 35.6254871,
          "longitude": 139.7194724
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254522,
            "longitude": 139.71945019999998
          },
          "ne": {
            "latitude": 35.6255293,
            "longitude": 139.7195011
          }
        },
        "planeHeightAtCenterMeters": 17.356289
      },
      {
        "pitchDegrees": 53.657974,
        "azimuthDegrees": 317.38953,
        "stats": {
          "areaMeters2": 15.558447,
          "sunshineQuantiles": [
            576.6988,
            625.5186,
            639.56006,
            659.1128,
            676.0762,
            694.4411,
            709.48535,
            724.6015,
            743.7173,
            772.57355,
            878.7667
          ],
          "groundAreaMeters2": 9.22
        },
        "center": {
          "latitude": 35.625523799999996,
          "longitude": 139.7195303
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254998,
            "longitude": 139.7195161
          },
          "ne": {
            "latitude": 35.6255478,
            "longitude": 139.71954449999998
          }
        },
        "planeHeightAtCenterMeters": 20.03914
      },
      {
        "pitchDegrees": 51.981155,
        "azimuthDegrees": 29.015461,
        "stats": {
          "areaMeters2": 14.108961,
          "sunshineQuantiles": [
            400.7259,
            551.2583,
            615.09674,
            648.95105,
            670.495,
            680.27875,
            693.66986,
            711.0767,
            743.88416,
            789.08026,
            873.2287
          ],
          "groundAreaMeters2": 8.69
        },
        "center": {
          "latitude": 35.6255132,
          "longitude": 139.7196387
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.625499,
            "longitude": 139.71961579999999
          },
          "ne": {
            "latitude": 35.625531099999996,
            "longitude": 139.7196682
          }
        },
        "planeHeightAtCenterMeters": 18.22325
      },
      {
        "pitchDegrees": 44.694256,
        "azimuthDegrees": 6.7292957,
        "stats": {
          "areaMeters2": 12.041583,
          "sunshineQuantiles": [
            546.6846,
            583.0872,
            616.6149,
            652.1643,
            683.4418,
            699.78436,
            713.77405,
            724.972,
            732.96643,
            745.2819,
            813.7918
          ],
          "groundAreaMeters2": 8.56
        },
        "center": {
          "latitude": 35.6255367,
          "longitude": 139.71955880000002
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6255252,
            "longitude": 139.7195382
          },
          "ne": {
            "latitude": 35.6255501,
            "longitude": 139.7195827
          }
        },
        "planeHeightAtCenterMeters": 19.058964
      },
      {
        "pitchDegrees": 11.552957,
        "azimuthDegrees": 171.90712,
        "stats": {
          "areaMeters2": 7.144753,
          "sunshineQuantiles": [
            588.4132,
            697.85223,
            762.3663,
            825.35345,
            989.8338,
            1152.4801,
            1212.2258,
            1224.0583,
            1234.229,
            1250.5597,
            1268.8527
          ],
          "groundAreaMeters2": 7
        },
        "center": {
          "latitude": 35.6254224,
          "longitude": 139.7195706
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.625408199999995,
            "longitude": 139.7195532
          },
          "ne": {
            "latitude": 35.6254401,
            "longitude": 139.7195846
          }
        },
        "planeHeightAtCenterMeters": 31.719898
      },
      {
        "pitchDegrees": 53.06303,
        "azimuthDegrees": 34.37954,
        "stats": {
          "areaMeters2": 10.433722,
          "sunshineQuantiles": [
            450.8436,
            505.3766,
            525.60596,
            539.85736,
            567.8421,
            584.2598,
            592.26166,
            602.13,
            614.30676,
            647.6517,
            736.38336
          ],
          "groundAreaMeters2": 6.27
        },
        "center": {
          "latitude": 35.6254734,
          "longitude": 139.719625
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254584,
            "longitude": 139.7196076
          },
          "ne": {
            "latitude": 35.625490299999996,
            "longitude": 139.71964119999998
          }
        },
        "planeHeightAtCenterMeters": 24.98067
      },
      {
        "pitchDegrees": 49.972607,
        "azimuthDegrees": 34.933434,
        "stats": {
          "areaMeters2": 9.406771,
          "sunshineQuantiles": [
            419.17184,
            514.7369,
            544.8989,
            566.9348,
            575.9047,
            588.0421,
            609.79517,
            620.3903,
            626.7771,
            644.6486,
            739.3668
          ],
          "groundAreaMeters2": 6.05
        },
        "center": {
          "latitude": 35.6254898,
          "longitude": 139.7196391
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254738,
            "longitude": 139.7196184
          },
          "ne": {
            "latitude": 35.625504,
            "longitude": 139.71965749999998
          }
        },
        "planeHeightAtCenterMeters": 21.72971
      },
      {
        "pitchDegrees": 3.2222903,
        "azimuthDegrees": 214.87424,
        "stats": {
          "areaMeters2": 4.7775536,
          "sunshineQuantiles": [
            409.02567,
            437.80313,
            492.20953,
            606.7533,
            743.7882,
            783.53564,
            799.8374,
            811.65405,
            815.95337,
            822.2627,
            844.44336
          ],
          "groundAreaMeters2": 4.77
        },
        "center": {
          "latitude": 35.6254454,
          "longitude": 139.7196155
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.625433,
            "longitude": 139.7196004
          },
          "ne": {
            "latitude": 35.6254586,
            "longitude": 139.7196339
          }
        },
        "planeHeightAtCenterMeters": 27.696573
      },
      {
        "pitchDegrees": 46.52361,
        "azimuthDegrees": 29.121042,
        "stats": {
          "areaMeters2": 6.4675,
          "sunshineQuantiles": [
            530.52386,
            577.28046,
            595.21466,
            717.26794,
            726.42725,
            738.7336,
            749.3561,
            758.4523,
            764.84784,
            774.215,
            791.14825
          ],
          "groundAreaMeters2": 4.45
        },
        "center": {
          "latitude": 35.6255135,
          "longitude": 139.7195791
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6255048,
            "longitude": 139.7195662
          },
          "ne": {
            "latitude": 35.6255258,
            "longitude": 139.7195941
          }
        },
        "planeHeightAtCenterMeters": 21.890022
      },
      {
        "pitchDegrees": 57.980034,
        "azimuthDegrees": 347.78787,
        "stats": {
          "areaMeters2": 8.128781,
          "sunshineQuantiles": [
            569.5991,
            591.93365,
            600.0812,
            615.21136,
            629.77356,
            651.6552,
            676.4091,
            699.52734,
            718.9332,
            740.2496,
            834.22864
          ],
          "groundAreaMeters2": 4.31
        },
        "center": {
          "latitude": 35.6255051,
          "longitude": 139.71955599999998
        },
        "boundingBox": {
          "sw": {
            "latitude": 35.6254937,
            "longitude": 139.7195409
          },
          "ne": {
            "latitude": 35.6255166,
            "longitude": 139.71956889999998
          }
        },
        "planeHeightAtCenterMeters": 23.41692
      }
    ],
    "solarPanelConfigs": [
      {
        "panelsCount": 4,
        "yearlyEnergyDcKwh": 1020.68787,
        "roofSegmentSummaries": [
          {
            "pitchDegrees": 1.9404444,
            "azimuthDegrees": 315.9278,
            "panelsCount": 2,
            "yearlyEnergyDcKwh": 448.79755,
            "segmentIndex": 1
          },
          {
            "pitchDegrees": 11.552957,
            "azimuthDegrees": 171.90712,
            "panelsCount": 2,
            "yearlyEnergyDcKwh": 571.8903,
            "segmentIndex": 10
          }
        ]
      },
      {
        "panelsCount": 5,
        "yearlyEnergyDcKwh": 1234.0469,
        "roofSegmentSummaries": [
          {
            "pitchDegrees": 1.9404444,
            "azimuthDegrees": 315.9278,
            "panelsCount": 3,
            "yearlyEnergyDcKwh": 662.15656,
            "segmentIndex": 1
          },
          {
            "pitchDegrees": 11.552957,
            "azimuthDegrees": 171.90712,
            "panelsCount": 2,
            "yearlyEnergyDcKwh": 571.8903,
            "segmentIndex": 10
          }
        ]
      },
      {
        "panelsCount": 6,
        "yearlyEnergyDcKwh": 1446.8296,
        "roofSegmentSummaries": [
          {
            "pitchDegrees": 1.9404444,
            "azimuthDegrees": 315.9278,
            "panelsCount": 4,
            "yearlyEnergyDcKwh": 874.9393,
            "segmentIndex": 1
          },
          {
            "pitchDegrees": 11.552957,
            "azimuthD<message truncated>

*/
