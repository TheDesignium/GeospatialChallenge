using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class travelAPI : MonoBehaviour
{

	public GeospatialController geo;
	public geoDetail geodetail;

	public string api = "259d76f42d584489a81394483c37c981";
	public string baseURL = "https://api.geoapify.com/v2/places?categories=";
	public string catagory = "amenity.toilet";
	public string condition = "none";
	public string lon = "139.624549";
	public string lat = "35.70086";
	public string radius = "1000";
	public string limit = "20";

	public Types _types;
	public Conditions _conds;

	public Dropdown _drop;
	public Dropdown _cond;

	public bool useLocation;
	public bool debugging;

	public List<string> nameList = new List<string>();
	public List<double> latitudesList = new List<double>();
	public List<double> longitudesList = new List<double>();
	List<string> tempList = new List<string>();

	public Texture2D _tex;

	public GameObject typeButtons;


//https://api.geoapify.com/v2/places?categories=amenity.toilet&filter=circle:139.624549,35.70086,1000&bias=proximity:139.624549,35.70086&limit=20&apiKey=YOUR_API_KEY
//https://api.geoapify.com/v1/icon/?type=circle&color=red&size=xx-large&icon=toilet&iconType=awesome&noShadow&scaleFactor=2&apiKey=YOUR_API_KEY
    void Start()
    {
			_drop.options.Clear ();
			string[] TypeNames = System.Enum.GetNames (typeof(Types));
      for(int i = 0; i < TypeNames.Length; i++){
          string _str = TypeNames[i].ToString();
					_str = _str.Replace("_s_", ".");
					_str = _str.Replace("_d_", "-");
          _str = _str.Replace("_", " ");
          _drop.options.Add (new Dropdown.OptionData() {text=_str});
      }

			_cond.options.Clear ();
			string[] condNames = System.Enum.GetNames (typeof(Conditions));
			for(int i = 0; i < condNames.Length; i++){
					string _str = condNames[i].ToString();
					_str = _str.Replace("_s_", ".");
					_str = _str.Replace("_d_", "-");
					_str = _str.Replace("_", " ");
					_cond.options.Add (new Dropdown.OptionData() {text=_str});
			}

			//StartCoroutine(getTexture());
    }


    void Update()
    {
      if(Input.GetKeyUp(KeyCode.Alpha7))
      {
        returnDetails();
      }
    }

	public void returnDetails()
  {
    StartCoroutine(GetDetails(true));
  }

	public void toggleLocation(bool b)
	{
		useLocation = b;
	}

	public IEnumerator GetDetails(bool createObjects)
  {
		if(useLocation == true)
		{
			List<string> lllist = geo.returnLatLon();
			lat = lllist[0];
			lon = lllist[1];
		}

		string url = baseURL;
		//"&conditions=wheelchair"
		if(condition != "none")
		{
			url = baseURL + catagory + "&conditions=" + condition + "&filter=circle:" + lon + "," + lat + "," + radius + "&bias=proximity:" + lon + "," + lat + "&limit=" + limit + "&apiKey=" + api;
		}
		else
		{
			url = baseURL + catagory + "&filter=circle:" + lon + "," + lat + "," + radius + "&bias=proximity:" + lon + "," + lat + "&limit=" + limit + "&apiKey=" + api;
		}

		Debug.Log(url);

		UnityWebRequest www = UnityWebRequest.Get(url);
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			 Debug.Log(www.error);
		}
		else
		{
			 // Show results as text
			string str = www.downloadHandler.text;
			Debug.Log(str);
			yield return StartCoroutine(parseDetails(str, createObjects));
		}
	}

	IEnumerator parseDetails(string jsonText, bool createObjects)
	{
		latitudesList.Clear();
		longitudesList.Clear();
		nameList.Clear();
        // Parse JSON data using Newtonsoft JSON
        JObject json = JObject.Parse(jsonText);
        JArray features = (JArray)json["features"];

				//Debug.Log(jsonText);

        // Loop through the features array
        foreach (JToken feature in features)
        {
            // Access properties and geometry of each feature
            JObject properties = (JObject)feature["properties"];
            JObject geometry = (JObject)feature["geometry"];

            // Access specific properties
						string name = (string)properties["name"];
            string country = (string)properties["country"];
            string city = (string)properties["city"];
            double lat = (double)geometry["coordinates"][1];
            double lon = (double)geometry["coordinates"][0];

			if(debugging == true)
			{
				Debug.Log("Country: " + country);
				Debug.Log("City: " + city);
				Debug.Log("Latitude: " + lat);
				Debug.Log("Longitude: " + lon);
			}
#if !UNITY_EDITOR
			if(createObjects == true)
			{
				geodetail.setAnchor(lat.ToString(), lon.ToString());
			}
#endif
			latitudesList.Add(lat);
			longitudesList.Add(lon);
			nameList.Add(name);
			yield return new WaitForEndOfFrame();

      }

		yield return new WaitForSeconds(1);
	}

	public void changeType(int _i)
	{
		_types = (Types)_i;
		string cat = _types.ToString();
		cat = cat.Replace("_s_", ".");
		cat = cat.Replace("_d_", "-");
		catagory = cat;
		StartCoroutine(getTexture());
	}

	public void changeCondition(int _i)
	{
		_conds = (Conditions)_i;
		string cat = _conds.ToString();
		cat = cat.Replace("_s_", ".");
		cat = cat.Replace("_d_", "-");
		condition = cat;
	}

	IEnumerator getTexture()
	{
		tempList.Clear();
		string cat = catagory;
		if(cat.Contains("."))
		{
			tempList = cat.Split('.').ToList();
			cat = tempList[tempList.Count - 1];
		}
		if(cat.Contains("_"))
		{
			tempList.Clear();
			tempList = cat.Split('_').ToList();
			cat = tempList[tempList.Count - 1];
		}

		string imgurl = "https://api.geoapify.com/v1/icon/?type=circle&color=red&size=xx-large&icon=" +
		cat + "&iconType=awesome&noShadow&scaleFactor=2&apiKey=" + api;

		Debug.Log(cat);

		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imgurl))
		{
				yield return uwr.SendWebRequest();

				if (uwr.result != UnityWebRequest.Result.Success)
				{
						Debug.Log(uwr.error);
				}
				else
				{
						var texture = DownloadHandlerTexture.GetContent(uwr);
						_tex = texture;
				}
		}
	}

	public void catagoryManual(int i)
	{
		StartCoroutine(manualSet(i));
	}

	IEnumerator manualSet(int i)
	{
		string cat = catagory;

		if(i == 0)
		{
			catagory = "commercial.convenience";
			cat = "store";
		}
		if(i == 1)
		{
			catagory = "amenity.toilet";
			cat = "toilet";
		}
		if(i == 2)
		{
			catagory = "commercial.books";
			cat = "book";
		}
		if(i == 3)
		{
			catagory = "public_transport.subway";
			cat = "subway";
		}
		if(i == 4)
		{
			catagory = "public_transport.bus";
			cat = "bus";
		}
		if(i == 5)
		{
			catagory = "tourism";
			cat = "landmark";
		}

		string imgurl = "https://api.geoapify.com/v1/icon/?type=circle&color=red&size=xx-large&icon=" +
		cat + "&iconType=awesome&noShadow&scaleFactor=2&apiKey=" + api;

		Debug.Log(cat);

		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imgurl))
		{
				yield return uwr.SendWebRequest();

				if (uwr.result != UnityWebRequest.Result.Success)
				{
						Debug.Log(uwr.error);
				}
				else
				{
						var texture = DownloadHandlerTexture.GetContent(uwr);
						_tex = texture;
				}
		}

		typeButtons.SetActive(false);
	}

	public enum Types {accommodation,
	accommodation_s_hotel,
	accommodation_s_hut,
	accommodation_s_apartment,
	accommodation_s_chalet,
	accommodation_s_guest_house,
	accommodation_s_hostel,
	accommodation_s_motel,
	activity,
	activity_s_community_center,
	activity_s_sport_club,
	commercial,
	commercial_s_supermarket,
	commercial_s_marketplace,
	commercial_s_shopping_mall,
	commercial_s_department_store,
	commercial_s_elektronics,
	commercial_s_outdoor_and_sport,
	commercial_s_outdoor_and_sport_s_water_sports,
	commercial_s_outdoor_and_s_s_port_s_ski,
	commercial_s_outdoor_and_sport_s_diving,
	commercial_s_outdoor_and_sport_s_hunting,
	commercial_s_outdoor_and_sport_s_bicycle,
	commercial_s_outdoor_and_sport_s_fishing,
	commercial_s_outdoor_and_sport_s_golf,
	commercial_s_vehicle,
	commercial_s_hobby,
	commercial_s_hobby_s_model,
	commercial_s_hobby_s_anime,
	commercial_s_hobby_s_collecting,
	commercial_s_hobby_s_games,
	commercial_s_hobby_s_brewing,
	commercial_s_hobby_s_photo,
	commercial_s_hobby_s_music,
	commercial_s_hobby_s_sewing_and_knitting,
	commercial_s_hobby_s_art,
	commercial_s_books,
	commercial_s_gift_and_souvenir,
	commercial_s_stationery,
	commercial_s_newsagent,
	commercial_s_tickets_and_lottery,
	commercial_s_clothing,
	commercial_s_clothing_s_shoes,
	commercial_s_clothing_s_clothes,
	commercial_s_clothing_s_underwear,
	commercial_s_clothing_s_sport,
	commercial_s_clothing_s_men,
	commercial_s_clothing_s_women,
	commercial_s_clothing_s_kids,
	commercial_s_clothing_s_accessories,
	commercial_s_bag,
	commercial_s_baby_goods,
	commercial_s_agrarian,
	commercial_s_garden,
	commercial_s_houseware_and_hardware,
	commercial_s_houseware_and_hardware_s_doityourself,
	commercial_s_houseware_and_hardware_s_hardware_and_tools,
	commercial_s_houseware_and_hardware_s_building_materials,
	commercial_s_houseware_and_hardware_s_building_materials_s_paint,
	commercial_s_houseware_and_hardware_s_building_materials_s_glaziery,
	commercial_s_houseware_and_hardware_s_building_materials_s_doors,
	commercial_s_houseware_and_hardware_s_building_materials_s_tiles,
	commercial_s_houseware_and_hardware_s_building_materials_s_windows,
	commercial_s_houseware_and_hardware_s_building_materials_s_flooring,
	commercial_s_houseware_and_hardware_s_fireplace,
	commercial_s_houseware_and_hardware_s_swimming_pool,
	commercial_s_florist,
	commercial_s_furniture_and_interior,
	commercial_s_furniture_and_interior_s_lighting,
	commercial_s_furniture_and_interior_s_curtain,
	commercial_s_furniture_and_interior_s_carpet,
	commercial_s_furniture_and_interior_s_kitchen,
	commercial_s_furniture_and_interior_s_bed,
	commercial_s_furniture_and_interior_s_bathroom,
	commercial_s_chemist,
	commercial_s_health_and_beauty,
	commercial_s_health_and_beauty_s_pharmacy,
	commercial_s_health_and_beauty_s_optician,
	commercial_s_health_and_beauty_s_medical_supply,
	commercial_s_health_and_beauty_s_hearing_aids,
	commercial_s_health_and_beauty_s_herbalist,
	commercial_s_health_and_beauty_s_cosmetics,
	commercial_s_health_and_beauty_s_wigs,
	commercial_s_toy_and_game,
	commercial_s_pet,
	commercial_s_food_and_drink,
	commercial_s_food_and_drink_s_bakery,
	commercial_s_food_and_drink_s_deli,
	commercial_s_food_and_drink_s_frozen_food,
	commercial_s_food_and_drink_s_pasta,
	commercial_s_food_and_drink_s_spices,
	commercial_s_food_and_drink_s_organic,
	commercial_s_food_and_drink_s_honey,
	commercial_s_food_and_drink_s_rice,
	commercial_s_food_and_drink_s_nuts,
	commercial_s_food_and_drink_s_health_food,
	commercial_s_food_and_drink_s_ice_cream,
	commercial_s_food_and_drink_s_seafood,
	commercial_s_food_and_drink_s_fruit_and_vegetable,
	commercial_s_food_and_drink_s_farm,
	commercial_s_food_and_drink_s_confectionery,
	commercial_s_food_and_drink_s_chocolate,
	commercial_s_food_and_drink_s_butcher,
	commercial_s_food_and_drink_s_cheese_and_dairy,
	commercial_s_food_and_drink_s_drinks,
	commercial_s_food_and_drink_s_coffee_and_tea,
	commercial_s_convenience,
	commercial_s_discount_store,
	commercial_s_smoking,
	commercial_s_second_hand,
	commercial_s_gas,
	commercial_s_weapons,
	commercial_s_pyrotechnics,
	commercial_s_energy,
	commercial_s_wedding,
	commercial_s_jewelry,
	commercial_s_watches,
	commercial_s_art,
	commercial_s_antiques,
	commercial_s_video_and_music,
	commercial_s_erotic,
	commercial_s_trade,
	commercial_s_kiosk,
	catering,
	catering_s_restaurant,
	catering_s_restaurant_s_pizza,
	catering_s_restaurant_s_burger,
	catering_s_restaurant_s_regional,
	catering_s_restaurant_s_italian,
	catering_s_restaurant_s_chinese,
	catering_s_restaurant_s_sandwich,
	catering_s_restaurant_s_chicken,
	catering_s_restaurant_s_mexican,
	catering_s_restaurant_s_japanese,
	catering_s_restaurant_s_american,
	catering_s_restaurant_s_kebab,
	catering_s_restaurant_s_indian,
	catering_s_restaurant_s_asian,
	catering_s_restaurant_s_sushi,
	catering_s_restaurant_s_french,
	catering_s_restaurant_s_german,
	catering_s_restaurant_s_thai,
	catering_s_restaurant_s_greek,
	catering_s_restaurant_s_seafood,
	catering_s_restaurant_s_fish_and_chips,
	catering_s_restaurant_s_steak_house,
	catering_s_restaurant_s_international,
	catering_s_restaurant_s_tex_d_mex,
	catering_s_restaurant_s_vietnamese,
	catering_s_restaurant_s_turkish,
	catering_s_restaurant_s_korean,
	catering_s_restaurant_s_noodle,
	catering_s_restaurant_s_barbecue,
	catering_s_restaurant_s_spanish,
	catering_s_restaurant_s_fish,
	catering_s_restaurant_s_ramen,
	catering_s_restaurant_s_mediterranean,
	catering_s_restaurant_s_friture,
	catering_s_restaurant_s_beef_bowl,
	catering_s_restaurant_s_lebanese,
	catering_s_restaurant_s_wings,
	catering_s_restaurant_s_georgian,
	catering_s_restaurant_s_tapas,
	catering_s_restaurant_s_indonesian,
	catering_s_restaurant_s_arab,
	catering_s_restaurant_s_portuguese,
	catering_s_restaurant_s_russian,
	catering_s_restaurant_s_filipino,
	catering_s_restaurant_s_african,
	catering_s_restaurant_s_malaysian,
	catering_s_restaurant_s_caribbean,
	catering_s_restaurant_s_peruvian,
	catering_s_restaurant_s_bavarian,
	catering_s_restaurant_s_brazilian,
	catering_s_restaurant_s_curry,
	catering_s_restaurant_s_dumpling,
	catering_s_restaurant_s_persian,
	catering_s_restaurant_s_argentinian,
	catering_s_restaurant_s_oriental,
	catering_s_restaurant_s_balkan,
	catering_s_restaurant_s_moroccan,
	catering_s_restaurant_s_pita,
	catering_s_restaurant_s_ethiopian,
	catering_s_restaurant_s_taiwanese,
	catering_s_restaurant_s_latin_american,
	catering_s_restaurant_s_hawaiian,
	catering_s_restaurant_s_irish,
	catering_s_restaurant_s_austrian,
	catering_s_restaurant_s_croatian,
	catering_s_restaurant_s_danish,
	catering_s_restaurant_s_tacos,
	catering_s_restaurant_s_bolivian,
	catering_s_restaurant_s_hungarian,
	catering_s_restaurant_s_western,
	catering_s_restaurant_s_european,
	catering_s_restaurant_s_jamaican,
	catering_s_restaurant_s_cuban,
	catering_s_restaurant_s_soup,
	catering_s_restaurant_s_uzbek,
	catering_s_restaurant_s_nepalese,
	catering_s_restaurant_s_czech,
	catering_s_restaurant_s_syrian,
	catering_s_restaurant_s_afghan,
	catering_s_restaurant_s_malay,
	catering_s_restaurant_s_chili,
	catering_s_restaurant_s_belgian,
	catering_s_restaurant_s_ukrainian,
	catering_s_restaurant_s_swedish,
	catering_s_restaurant_s_pakistani,
	catering_s_fast_food,
	catering_s_fast_food_s_pizza,
	catering_s_fast_food_s_burger,
	catering_s_fast_food_s_sandwich,
	catering_s_fast_food_s_kebab,
	catering_s_fast_food_s_fish_and_chips,
	catering_s_fast_food_s_noodle,
	catering_s_fast_food_s_ramen,
	catering_s_fast_food_s_wings,
	catering_s_fast_food_s_tapas,
	catering_s_fast_food_s_pita,
	catering_s_fast_food_s_tacos,
	catering_s_fast_food_s_soup,
	catering_s_fast_food_s_salad,
	catering_s_fast_food_s_hot_dog,
	catering_s_cafe,
	catering_s_cafe_s_waffle,
	catering_s_cafe_s_ice_cream,
	catering_s_cafe_s_coffee_shop,
	catering_s_cafe_s_donut,
	catering_s_cafe_s_crepe,
	catering_s_cafe_s_bubble_tea,
	catering_s_cafe_s_cake,
	catering_s_cafe_s_frozen_yogurt,
	catering_s_cafe_s_dessert,
	catering_s_cafe_s_coffee,
	catering_s_cafe_s_tea,
	catering_s_food_court,
	catering_s_bar,
	catering_s_pub,
	catering_s_ice_cream,
	catering_s_biergarten,
	catering_s_taproom,
	education,
	education_s_school,
	education_s_driving_school,
	education_s_music_school,
	education_s_language_school,
	education_s_library,
	education_s_college,
	education_s_university,
	childcare,
	childcare_s_kindergarten,
	entertainment,
	entertainment_s_culture,
	entertainment_s_culture_s_theatre,
	entertainment_s_culture_s_arts_centre,
	entertainment_s_culture_s_gallery,
	entertainment_s_zoo,
	entertainment_s_aquarium,
	entertainment_s_planetarium,
	entertainment_s_museum,
	entertainment_s_cinema,
	entertainment_s_amusement_arcade,
	entertainment_s_escape_game,
	entertainment_s_miniature_golf,
	entertainment_s_bowling_alley,
	entertainment_s_flying_fox,
	entertainment_s_theme_park,
	entertainment_s_water_park,
	entertainment_s_activity_park,
	entertainment_s_activity_park_s_trampoline,
	entertainment_s_activity_park_s_climbing,
	healthcare,
	healthcare_s_clinic_or_praxis,
	healthcare_s_clinic_or_praxis_s_allergology,
	healthcare_s_clinic_or_praxis_s_vascular_surgery,
	healthcare_s_clinic_or_praxis_s_urology,
	healthcare_s_clinic_or_praxis_s_trauma,
	healthcare_s_clinic_or_praxis_s_rheumatology,
	healthcare_s_clinic_or_praxis_s_radiology,
	healthcare_s_clinic_or_praxis_s_pulmonology,
	healthcare_s_clinic_or_praxis_s_psychiatry,
	healthcare_s_clinic_or_praxis_s_paediatrics,
	healthcare_s_clinic_or_praxis_s_otolaryngology,
	healthcare_s_clinic_or_praxis_s_orthopaedics,
	healthcare_s_clinic_or_praxis_s_ophthalmology,
	healthcare_s_clinic_or_praxis_s_occupational,
	healthcare_s_clinic_or_praxis_s_gynaecology,
	healthcare_s_clinic_or_praxis_s_general,
	healthcare_s_clinic_or_praxis_s_gastroenterology,
	healthcare_s_clinic_or_praxis_s_endocrinology,
	healthcare_s_clinic_or_praxis_s_dermatology,
	healthcare_s_clinic_or_praxis_s_cardiology,
	healthcare_s_dentist,
	healthcare_s_dentist_s_orthodontics,
	healthcare_s_hospital,
	healthcare_s_pharmacy,
	heritage,
	heritage_s_unesco,
	leisure,
	leisure_s_picnic,
	leisure_s_picnic_s_picnic_site,
	leisure_s_picnic_s_picnic_table,
	leisure_s_picnic_s_bbq,
	leisure_s_playground,
	leisure_s_spa,
	leisure_s_spa_s_public_bath,
	leisure_s_spa_s_sauna,
	leisure_s_park,
	leisure_s_park_s_garden,
	leisure_s_park_s_nature_reserve,
	man_made,
	man_made_s_pier,
	man_made_s_breakwater,
	man_made_s_tower,
	man_made_s_water_tower,
	man_made_s_bridge,
	man_made_s_lighthouse,
	man_made_s_windmill,
	man_made_s_watermill,
	natural,
	natural_s_forest,
	natural_s_water,
	natural_s_water_s_spring,
	natural_s_water_s_reef,
	natural_s_water_s_hot_spring,
	natural_s_water_s_geyser,
	natural_s_water_s_sea,
	natural_s_mountain,
	natural_s_mountain_s_peak,
	natural_s_mountain_s_glacier,
	natural_s_mountain_s_cliff,
	natural_s_mountain_s_rock,
	natural_s_mountain_s_cave_entrance,
	natural_s_sand,
	natural_s_sand_s_dune,
	natural_s_protected_area,
	national_park,
	office,
	office_s_government,
	office_s_government_s_administrative,
	office_s_government_s_register_office,
	office_s_government_s_tax,
	office_s_government_s_public_service,
	office_s_government_s_ministry,
	office_s_government_s_healthcare,
	office_s_government_s_prosecutor,
	office_s_government_s_transportation,
	office_s_government_s_social_services,
	office_s_government_s_legislative,
	office_s_government_s_education,
	office_s_government_s_customs,
	office_s_government_s_social_security,
	office_s_government_s_environment,
	office_s_government_s_migration,
	office_s_government_s_cadaster,
	office_s_government_s_forestry,
	office_s_government_s_agriculture,
	office_s_company,
	office_s_estate_agent,
	office_s_insurance,
	office_s_lawyer,
	office_s_telecommunication,
	office_s_educational_institution,
	office_s_association,
	office_s_non_profit,
	office_s_diplomatic,
	office_s_it,
	office_s_accountant,
	office_s_employment_agency,
	office_s_religion,
	office_s_research,
	office_s_architect,
	office_s_financial,
	office_s_tax_advisor,
	office_s_advertising_agency,
	office_s_notary,
	office_s_newspaper,
	office_s_political_party,
	office_s_logistics,
	office_s_energy_supplier,
	office_s_travel_agent,
	office_s_financial_advisor,
	office_s_consulting,
	office_s_foundation,
	office_s_coworking,
	office_s_water_utility,
	office_s_forestry,
	office_s_charity,
	office_s_security,
	parking,
	parking_s_cars,
	parking_s_cars_s_surface,
	parking_s_cars_s_multistorey,
	parking_s_cars_s_underground,
	parking_s_cars_s_rooftop,
	parking_s_surface,
	parking_s_multistorey,
	parking_s_underground,
	parking_s_rooftop,
	parking_s_motorcycle,
	parking_s_bicycles,
	pet,
	pet_s_shop,
	pet_s_veterinary,
	pet_s_service,
	pet_s_dog_park,
	rental,
	rental_s_car,
	rental_s_storage,
	rental_s_bicycle,
	rental_s_boat,
	rental_s_ski,
	service,
	service_s_financial,
	service_s_financial_s_atm,
	service_s_financial_s_payment_terminal,
	service_s_financial_s_bank,
	service_s_financial_s_bureau_de_change,
	service_s_financial_s_money_transfer,
	service_s_financial_s_money_lender,
	service_s_cleaning,
	service_s_cleaning_s_lavoir,
	service_s_cleaning_s_laundry,
	service_s_cleaning_s_dry_cleaning,
	service_s_travel_agency,
	service_s_post,
	service_s_post_s_office,
	service_s_post_s_box,
	service_s_police,
	service_s_vehicle,
	service_s_vehicle_s_fuel,
	service_s_vehicle_s_car_wash,
	service_s_vehicle_s_charging_station,
	service_s_vehicle_s_repair,
	service_s_vehicle_s_repair_s_car,
	service_s_vehicle_s_repair_s_motorcycle,
	service_s_beauty,
	service_s_beauty_s_hairdresser,
	service_s_beauty_s_spa,
	service_s_beauty_s_massage,
	service_s_tailor,
	service_s_funeral_directors,
	service_s_bookmaker,
	service_s_estate_agent,
	service_s_locksmith,
	service_s_taxi,
	service_s_social_facility,
	service_s_social_facility_s_shelter,
	service_s_social_facility_s_food,
	service_s_social_facility_s_clothers,
	tourism,
	tourism_s_information,
	tourism_s_information_s_office,
	tourism_s_information_s_map,
	tourism_s_information_s_ranger_station,
	tourism_s_attraction,
	tourism_s_attraction_s_artwork,
	tourism_s_attraction_s_viewpoint,
	tourism_s_attraction_s_fountain,
	tourism_s_attraction_s_clock,
	tourism_s_sights,
	tourism_s_sights_s_place_of_worship,
	tourism_s_sights_s_place_of_worship_s_church,
	tourism_s_sights_s_place_of_worship_s_chapel,
	tourism_s_sights_s_place_of_worship_s_cathedral,
	tourism_s_sights_s_place_of_worship_s_mosque,
	tourism_s_sights_s_place_of_worship_s_synagogue,
	tourism_s_sights_s_place_of_worship_s_temple,
	tourism_s_sights_s_place_of_worship_s_shrine,
	tourism_s_sights_s_monastery,
	tourism_s_sights_s_city_hall,
	tourism_s_sights_s_conference_centre,
	tourism_s_sights_s_lighthouse,
	tourism_s_sights_s_windmill,
	tourism_s_sights_s_tower,
	tourism_s_sights_s_battlefield,
	tourism_s_sights_s_fort,
	tourism_s_sights_s_castle,
	tourism_s_sights_s_ruines,
	tourism_s_sights_s_archaeological_site,
	tourism_s_sights_s_city_gate,
	tourism_s_sights_s_bridge,
	tourism_s_sights_s_memorial,
	tourism_s_sights_s_memorial_s_aircraft,
	tourism_s_sights_s_memorial_s_locomotive,
	tourism_s_sights_s_memorial_s_railway_car,
	tourism_s_sights_s_memorial_s_ship,
	tourism_s_sights_s_memorial_s_tank,
	tourism_s_sights_s_memorial_s_tomb,
	tourism_s_sights_s_memorial_s_monument,
	tourism_s_sights_s_memorial_s_wayside_cross,
	tourism_s_sights_s_memorial_s_boundary_stone,
	tourism_s_sights_s_memorial_s_pillory,
	tourism_s_sights_s_memorial_s_milestone,
	religion,
	religion_s_place_of_worship,
	religion_s_place_of_worship_s_buddhism,
	religion_s_place_of_worship_s_christianity,
	religion_s_place_of_worship_s_hinduism,
	religion_s_place_of_worship_s_islam,
	religion_s_place_of_worship_s_judaism,
	religion_s_place_of_worship_s_shinto,
	religion_s_place_of_worship_s_sikhism,
	religion_s_place_of_worship_s_multifaith,
	camping,
	camping_s_camp_pitch,
	camping_s_camp_site,
	camping_s_summer_camp,
	camping_s_caravan_site,
	amenity,
	amenity_s_toilet,
	amenity_s_drinking_water,
	amenity_s_give_box,
	amenity_s_give_box_s_food,
	amenity_s_give_box_s_books,
	beach,
	beach_s_beach_resort,
	adult,
	adult_s_nightclub,
	adult_s_stripclub,
	adult_s_swingerclub,
	adult_s_brothel,
	adult_s_casino,
	adult_s_adult_gaming_centre,
	airport,
	airport_s_international,
	building,
	building_s_residential,
	building_s_commercial,
	building_s_industrial,
	building_s_office,
	building_s_catering,
	building_s_healthcare,
	building_s_university,
	building_s_college,
	building_s_dormitory,
	building_s_school,
	building_s_driving_school,
	building_s_kindergarten,
	building_s_public_and_civil,
	building_s_sport,
	building_s_spa,
	building_s_place_of_worship,
	building_s_holiday_house,
	building_s_accommodation,
	building_s_tourism,
	building_s_transportation,
	building_s_military,
	building_s_service,
	building_s_facility,
	building_s_garage,
	building_s_parking,
	building_s_toilet,
	building_s_prison,
	building_s_entertainment,
	building_s_historic,
	ski,
	ski_s_lift,
	ski_s_lift_s_cable_car,
	ski_s_lift_s_gondola,
	ski_s_lift_s_mixed_lift,
	ski_s_lift_s_chair_lift,
	ski_s_lift_s_tow_line,
	ski_s_lift_s_magic_carpet,
	sport,
	sport_s_stadium,
	sport_s_dive_centre,
	sport_s_horse_riding,
	sport_s_ice_rink,
	sport_s_pitch,
	sport_s_sports_centre,
	sport_s_swimming_pool,
	sport_s_track,
	sport_s_fitness,
	sport_s_fitness_s_fitness_centre,
	sport_s_fitness_s_fitness_station,
	public_transport,
	public_transport_s_train,
	public_transport_s_light_rail,
	public_transport_s_monorail,
	public_transport_s_subway,
	public_transport_s_subway_s_entrance,
	public_transport_s_bus,
	public_transport_s_tram,
	public_transport_s_ferry,
	public_transport_s_aerialway,
	administrative,
	administrative_s_continent_level,
	administrative_s_country_level,
	administrative_s_country_part_level,
	administrative_s_state_level,
	administrative_s_county_level,
	administrative_s_city_level,
	administrative_s_district_level,
	administrative_s_suburb_level,
	administrative_s_neighbourhood_level,
	postal_code,
	political,
	low_emission_zone,
	populated_place,
	populated_place_s_hamlet,
	populated_place_s_village,
	populated_place_s_neighbourhood,
	populated_place_s_suburb,
	populated_place_s_town,
	populated_place_s_city_block,
	populated_place_s_quarter,
	populated_place_s_city,
	populated_place_s_allotments,
	populated_place_s_county,
	populated_place_s_municipality,
	populated_place_s_district,
	populated_place_s_region,
	populated_place_s_state,
	populated_place_s_borough,
	populated_place_s_subdistrict,
	populated_place_s_province,
	populated_place_s_township,
	production,
	production_s_factory,
	production_s_winery,
	production_s_brewery,
	production_s_cheese,
	production_s_pottery}

		public enum Conditions {
			none,
			internet_access,
			internet_access_s_free,
			internet_access_s_for_customers,
			wheelchair,
			wheelchair_s_yes,
			wheelchair_s_limited,
			dogs,
			dogs_s_yes,
			dogs_s_leashed,
			no_d_dogs,
			access,
			access_s_yes,
			access_s_not_specified,
			access_limited,
			access_limited_s_private,
			access_limited_s_customers,
			access_limited_s_with_permit,
			access_limited_s_services,
			no_access,
			fee,
			no_fee,
			no_fee_s_no,
			no_fee_s_not_specified,
			named,
			vegetarian,
			vegetarian_s_only,
			vegan,
			vegan_s_only,
			halal,
			halal_s_only,
			kosher,
			kosher_s_only,
			organic,
			organic_s_only,
			gluten_free,
			sugar_free,
			egg_free,
			soy_free
		}

}


/*

{"type":"FeatureCollection","features":[{"type":"Feature","properties":{"country":"Japan","country_code":"jp","city":"Suginami","postcode":"166-0016","district":"Ogikubo 3-chome","street":"大谷戸橋","lon":139.6265522,"lat":35.6968304,"formatted":"大谷戸橋, Suginami, Ogikubo 3-chome 166-0016, Japan","address_line1":"大谷戸橋","address_line2":"Suginami, Ogikubo 3-chome 166-0016, Japan","categories":["amenity","amenity.toilet"],"details":[],"datasource":{"sourcename":"openstreetmap","attribution":"© OpenStreetMap contributors","license":"Open Database Licence","url":"https://www.openstreetmap.org/copyright","raw":{"osm_id":4946583009,"amenity":"toilets","osm_type":"n"}},"distance":482,"place_id":"51930733b70c74614059b2cc10bd31d94140f00103f901e1ddd62601000000"},"geometry":{"type":"Point","coordinates":[139.62655219999996,35.69683039970677]}},{"type":"Feature","properties":{"country":"Japan","country_code":"jp","city":"Suginami","postcode":"166-8570","district":"Naritahigashi 4-chome","suburb":"Koenji","street":"Ome-kaido Avenue","lon":139.629209,"lat":35.6983124,"formatted":"Ome-kaido Avenue, Koenji, Suginami, Naritahigashi 4-chome 166-8570, Japan","address_line1":"Ome-kaido Avenue","address_line2":"Koenji, Suginami, Naritahigashi 4-chome 166-8570, Japan","categories":["amenity","amenity.toilet"],"details":[],"datasource":{"sourcename":"openstreetmap","attribution":"© OpenStreetMap contributors","license":"Open Database Licence","url":"https://www.openstreetmap.org/copyright","raw":{"osm_id":3114548085,"amenity":"toilets","osm_type":"n"}},"distance":508,"place_id":"512aabe97a2274614059f090fb4c62d94140f00103f901753ba4b900000000"},"geometry":{"type":"Point","coordinates":[139.629209,35.69831239970688]}},{"type":"Feature","properties":{"country":"Japan","country_code":"jp","city":"Suginami","postcode":"167-0051","district":"Ogikubo 5-chome","street":"荻窪南口仲通商店街","lon":139.6208285,"lat":35.7043639,"formatted":"荻窪南口仲通商店街, Suginami, Ogikubo 5-chome 167-0051, Japan","address_line1":"荻窪南口仲通商店街","address_line2":"Suginami, Ogikubo 5-chome 167-0051, Japan","categories":["amenity","amenity.toilet","wheelchair","wheelchair.yes"],"details":["details.facilities"],"datasource":{"sourcename":"openstreetmap","attribution":"© OpenStreetMap contributors","license":"Open Database Licence","url":"https://www.openstreetmap.org/copyright","raw":{"level":-1,"osm_id":6423985794,"unisex":"yes","amenity":"toilets","osm_type":"n","wheelchair":"yes","changing_table":"yes","changing_table:location":"room"}},"distance":514,"place_id":"5197fdbad3dd73614059fedca49828da4140f00103f901823ee67e01000000"},"geometry":{"type":"Point","coordinates":[139.6208285,35.704363899707445]}},{"type":"Feature","properties":{"country":"Japan","country_code":"jp","city":"Suginami","postcode":"167-0052","district":"Ogikubo 5-chome","street":"善福寺川ジョギングコース","lon":139.6178412,"lat":35.700823,"formatted":"善福寺川ジョギングコース, Suginami, Ogikubo 5-chome 167-0052, Japan","address_line1":"善福寺川ジョギングコース","address_line2":"Suginami, Ogikubo 5-chome 167-0052, Japan","categories":["amenity","amenity.toilet"],"details":[],"datasource":{"sourcename":"openstreetmap","attribution":"© OpenStreetMap contributors","license":"Open Database Licence","url":"https://www.openstreetmap.org/copyright","raw":{"osm_id":1026500857,"amenity":"toilets","osm_type":"n"}},"distance":607,"place_id":"51e283e85ac5736140596d036c91b4d94140f00103f901f9282f3d00000000"},"geometry":{"type":"Point","coordinates":[139.6178412,35.700822999707135]}},{"type":"Feature","properties":{"country":"Japan","country_code":"jp","city":"Suginami","postcode":"166-8570","district":"Naritahigashi 4-chome","suburb":"Koenji","street":"Ome-kaido Avenue","lon":139.6339777,"lat":35.7014291,"formatted":"Ome-kaido Avenue, Koenji, Suginami, Naritahigashi 4-chome 166-8570, Japan","address_line1":"Ome-kaido Avenue","address_line2":"Koenji, Suginami, Naritahigashi 4-chome 166-8570, Japan","categories":["amenity","amenity.toilet"],"details":[],"datasource":{"sourcename":"openstreetmap","attribution":"© OpenStreetMap contributors","license":"Open Database Licence","url":"https://www.openstreetmap.org/copyright","raw":{"osm_id":1070078702,"amenity":"toilets","osm_type":"n"}},"distance":856,"place_id":"5196fc998b49746140593edac16dc8d94140f00103f901ee1ac83f00000000"},"geometry":{"type":"Point","coordinates":[139.6339777,35.70142909970717]}}]}

*/
