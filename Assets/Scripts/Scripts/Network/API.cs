using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Utils;
using SimpleJSON;
using Items;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles all the comunication between the game and the server
/// </summary>
public class API : MonoBehaviour {

    public enum RequestType { GET, POST}
    public class Request
    {
        private int _requestID;
        public int RequestID { get { return _requestID; } }

        private RequestType _type;
        public RequestType RequestMethod { get { return _type; } }

        private string _url;
        public string Url { get { return _url; } }

        private AUTH_TYPE _authType;
        public AUTH_TYPE AuthType { get { return _authType; } }

        private Dictionary<string, string> _parameters;
        public Dictionary<string, string> Parameters { get { return _parameters; } }

        private ResponseCallback _correctCallback;
        public ResponseCallback CorrectCallback { get { return _correctCallback; } }

        ResponseCallback _unhauthorizedCallback;
        public ResponseCallback UnhauthorizedCallback { get { return _unhauthorizedCallback; } }

        ResponseCallback _notFoundCallback;
        public ResponseCallback NotFoundCallback { get { return _notFoundCallback; } }

        ResponseCallback _serverErrorCallback;
        public ResponseCallback ServerErrorCallback { get { return _notFoundCallback; } }

        private WWWForm _form; 
        public WWWForm Form { get { return _form; } }

        public Request(int id, string url, AUTH_TYPE type, RequestType requestType)
        {
            _requestID = id;
            _type = requestType;
            _authType = type;
            _url = url;
            _parameters = null;

            _correctCallback = null;
            _unhauthorizedCallback = null;
            _notFoundCallback = null;
            _serverErrorCallback = null;
            _form = null;
        }

        public void AddParameters(Dictionary<string, string > parameters)
        {
            _parameters = parameters;

        }

        public Request AddCorrectCallback(ResponseCallback callback)
        {
            _correctCallback = callback;
            return this;
        }

        public Request AddUnhauzorizedCallback(ResponseCallback callback)
        {
            _unhauthorizedCallback = callback;
            return this;
        }

        public Request AddNotFoundCallback(ResponseCallback callback)
        {
            _notFoundCallback = callback;
            return this;
        }

        public Request AddServerErrorCallback(ResponseCallback callback)
        {
            _serverErrorCallback = callback;
            return this;
        }

        public Request AddForm(WWWForm form)
        {
            _form = form;
            return this;
        }
    }

    public enum AUTH_TYPE { CRED, SOFT, HARD };
    public const int RESPONSE_CODE_CORRECT = 200;
    public const int RESPONSE_CODE_UNAUTHORIZED = 401;
    public const int RESPONSE_CODE_NOT_FOUND = 404;
    public const int RESPONSE_CODE_SERVER_ERROR = 500;

    private UIElement loadingDialog;

	private string userEmail , userPassoword;
    private string userToken, hardToken, softToken;

    public delegate void ResponseCallback(WWW www);

    //This is for recognizing the id of each request
    public int requestID = 0;

    private List<Request> _requestsToDo;
    private Request _actualRequest;

    void Awake()
    {
        createLoadingDialog();
        DontDestroyOnLoad(gameObject);
        _requestsToDo = new List<Request>();
    }
    
    void Start()
    {
    }

    public void createLoadingDialog()
    {
        GameObject loadingInstance = (GameObject)Instantiate((GameObject)Resources.Load(Constants.PATH_RESOURCES + "UI/Loading"));
        loadingInstance.transform.SetParent(GameObject.Find("UI").transform);
        loadingInstance.transform.localPosition = new Vector3(0, 0, 0);
        loadingDialog = loadingInstance.GetComponent<UIElement>();
        loadingDialog.transform.Find("Text").GetComponent<Text>().text = TextFactory.GetText("api_loading_text");
        loadingDialog.hide();
    }

    public void ShowLoadingDialog()
    {
        try
        {
            loadingDialog.show();
        }
        catch (Exception ex)
        {
            createLoadingDialog();
            loadingDialog.show();
        }
        
    }

    public void HideLoadingDialog()
    {
        loadingDialog.hide();
    }


    void Update()
    {

    }

	public void login()
	{
        UILoginController.instance.hideLoginError();
        userEmail = GameObject.Find("txt_email").GetComponent<InputField>().text;
		userPassoword = GameObject.Find("txt_password").GetComponent<InputField>().text;

        string url = Constants.getAPIURL(Constants.API_AUTH_URL);

        loadingDialog.show();
		sendGETRequest (url, AUTH_TYPE.CRED, processValidLoginResponse, processInvalidLoginResponse, processInvalidLoginResponse, processInvalidLoginResponse);
    }

    private string getCredentialsForRequest(AUTH_TYPE authType, int reqID)
    {
        string header = "";
        switch (authType)
        {
            case AUTH_TYPE.CRED:
                header = "Basic " + Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(userEmail + ":" + userPassoword + ":" + reqID.ToString()));
                break;
            case AUTH_TYPE.HARD:
                header = "Basic " + Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(userToken + ":" + hardToken + ":" + reqID.ToString()));
                break;
            case AUTH_TYPE.SOFT:
                header = "Basic " + Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(userToken + ":" + softToken + ":" + reqID.ToString()));
                break;
            default:
                Debug.LogError("Unexpected Auth type");
                break;
        }
        return header;
    }

    public void sendGETRequest(string url, AUTH_TYPE authType, ResponseCallback correctCallback = null, ResponseCallback unhauthorizedHandler = null, ResponseCallback notFoundHandler = null, ResponseCallback serverErrorHandler = null){
        int reqID = generateRequestID();
        Dictionary<string, string> param = new Dictionary<string, string>();
        Request request = new Request(reqID, url, authType, RequestType.GET);

        if(correctCallback != null)
        {
            request.AddCorrectCallback(correctCallback);
        }

        if(unhauthorizedHandler != null)
        {
            request.AddUnhauzorizedCallback(unhauthorizedHandler);
        }

        if(notFoundHandler != null)
        {
            request.AddNotFoundCallback(notFoundHandler);
        }

        if(serverErrorHandler != null)
        {
            request.AddServerErrorCallback(serverErrorHandler);
        }

        string auth = getCredentialsForRequest(authType, reqID);
        //add autorization to the request

        param.Add("Authorization", auth);

        request.AddParameters(param);

        if (_actualRequest != null)
        {
            _requestsToDo.Add(request);
        }
        else
        {
            _actualRequest = request;
            PerformRequest();
        }

    }

    public void PerformRequest()
    {
        WWW www = null;

        if(_actualRequest.RequestMethod == RequestType.GET)
        {
            www = new WWW(_actualRequest.Url, null, _actualRequest.Parameters);
        }
        else
        {
            www = new WWW(_actualRequest.Url, _actualRequest.Form);
        }

        StartCoroutine(WaitForRequest(www, _actualRequest.AuthType));

    }

    /// <summary>
    /// Depending on what type of request we make probably we would need to store the new credentials 
    /// into our class
    /// </summary>
    /// <param name="www"></param>
    /// <param name="authType"></param>
    private void storeNewCredentials(WWW www, AUTH_TYPE authType)
    {
        if (Constants.USE_FAKE_API) return;

        var json = JSON.Parse(www.text);

        switch (authType)
        {
            case AUTH_TYPE.CRED:
                userToken = json["at"];
                hardToken = json["ht"];
                softToken = json["st"];
                break;
            case AUTH_TYPE.HARD:
                hardToken = json["ht"];
                break;
            case AUTH_TYPE.SOFT:
                softToken = json["st"];
                break;
            default:
                Debug.LogError("Received an unexpected type of request");
                break;
        }

        //Used to Debug webservice responses
        if (Constants.DEVELOPMENT)
        {
            Debugger.instance.addDebugginInfo("ht", hardToken);
            Debugger.instance.addDebugginInfo("at", userToken);
        }
    }

	private void processValidLoginResponse(WWW www){
        //TODO: continue this logic
        sendGETRequest(Constants.getAPIURL(Constants.API_GET_PLAYER_STATS_URL), AUTH_TYPE.HARD, setUserData);
    }

	private void processInvalidLoginResponse(WWW www){
        GameObject.Find("BadCredentials").GetComponent<UIElement>().show();
        loadingDialog.hide();
    }

	private bool isValidResponse(WWW www){
		return www.error == null || www.error == "";
	}

	
	public void POST(string url, Dictionary<string, string> post, AUTH_TYPE authType, ResponseCallback correctCallback = null, ResponseCallback unhauthorizedHandler = null, ResponseCallback notFoundHandler = null, ResponseCallback serverErrorHandler = null)
	{
        int reqID = generateRequestID();

        Request request = new Request(reqID, url, authType, RequestType.POST);

        if (correctCallback != null)
        {
            request.AddCorrectCallback(correctCallback);
        }

        if (unhauthorizedHandler != null)
        {
            request.AddUnhauzorizedCallback(unhauthorizedHandler);
        }

        if (notFoundHandler != null)
        {
            request.AddNotFoundCallback(notFoundHandler);
        }

        if (serverErrorHandler != null)
        {
            request.AddServerErrorCallback(serverErrorHandler);
        }

        WWWForm form = new WWWForm();

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        string auth = getCredentialsForRequest(authType, reqID);
        
        //add autorization to the request
        parameters.Add("Authorization", auth);

        foreach (KeyValuePair<String, String> post_arg in post)
		{
			form.AddField(post_arg.Key, post_arg.Value);
		}

        form.AddField("at", userToken);

        switch (authType)
        {
            case AUTH_TYPE.HARD:
                form.AddField("ht", hardToken);
                form.AddField("requestID", reqID);
                break;
            case AUTH_TYPE.SOFT:
                form.AddField("st", softToken);
                form.AddField("requestID", reqID);
                break;
            default:
                Debug.LogError("Unsoported auth type!"); 
                break;
        }

        request.AddParameters(parameters);
        request.AddForm(form);

        if(_actualRequest != null)
        {
            _requestsToDo.Add(request);
        }
        else
        {
            _actualRequest = request;
            PerformRequest();
        }

	}

    WWW handleResponse(WWW www, AUTH_TYPE authType)
    {
        JSONNode response = null;
        try
        {
            response = JSON.Parse(www.text);
        }
        catch(Exception ex)
        {
            if (Constants.DEVELOPMENT) Debug.Log(www.text);
            loadingDialog.hide();
            DialogManager.instance.CreateDialogRequest(new DialogManager.DialogRequest(Dialog.DIALOG_TYPE.ALERT, "Unexpected Server Error", "If the error persist please contact the administrator", null, null, true));
        }

        try
        {
            int requestId = response["requestID"].AsInt;

            switch (getResponseCode(www))
            {
                case RESPONSE_CODE_CORRECT:
                    storeNewCredentials(www, authType);
					if (_actualRequest.CorrectCallback != null)
                    {
                        _actualRequest.CorrectCallback(www);
                    }
                    else
                    {
                        if (Constants.DEVELOPMENT) Debug.Log("Correct Response");
                    }
                    break;
                case RESPONSE_CODE_UNAUTHORIZED:
					if (_actualRequest.UnhauthorizedCallback != null)
                    {
                        _actualRequest.UnhauthorizedCallback(www);
                    }
                    else
                    {
                        if (Constants.DEVELOPMENT) Debug.Log("Unhauzorised");
                    }
                    break;
                case RESPONSE_CODE_NOT_FOUND:
					if (_actualRequest.NotFoundCallback != null)
                    {
                        _actualRequest.NotFoundCallback(www);
                    }
                    else
                    {
                        if (Constants.DEVELOPMENT)
                        {
                            Debug.LogError("Resource not found:");
                            Debug.LogError(www.url);
                        }
                    }

                    break;
                case RESPONSE_CODE_SERVER_ERROR:
					if (_actualRequest.ServerErrorCallback != null)
                    {
                        _actualRequest.ServerErrorCallback(www);
                    }
                    else
                    {
                        if (Constants.DEVELOPMENT)
                        {
                            Debug.Log("Server Error");
                            Debug.Log(www.text);
                        }
                    }

                    break;
                default:
                    break;
            }
            if(_requestsToDo.Count > 0)
            {
                _actualRequest = _requestsToDo[0];
                _requestsToDo.RemoveAt(0);
                PerformRequest();
            }
            else
            {
                _actualRequest = null;
            }
            return www;
        }

        catch(Exception ex)
        {
            loadingDialog.hide();
            DialogManager.instance.CreateDialogRequest(new DialogManager.DialogRequest(Dialog.DIALOG_TYPE.ALERT, "Unexpected Server Error", "If the error persist please contact the administrator", null, null, true));
        }
        return null;
    }
    
	private IEnumerator WaitForRequest(WWW www, AUTH_TYPE authType)
	{
        yield return www;
        handleResponse(www, authType);
    }


    public static int getResponseCode(WWW request)
    {
        int ret = 0;
        if (request.responseHeaders == null)
        {
            Debug.LogError("no response headers.");
        }
        else {
            if (!request.responseHeaders.ContainsKey("STATUS"))
            {
                Debug.LogError("response headers has no STATUS.");
            }
            else {
                ret = parseResponseCode(request.responseHeaders["STATUS"]);
            }
        }

        return ret;
    }

    public static int parseResponseCode(string statusLine)
    {
        int ret = 0;

        string[] components = statusLine.Split(' ');
        if (components.Length < 3)
        {
            Debug.LogError("invalid response status: " + statusLine);
        }
        else {
            if (!int.TryParse(components[1], out ret))
            {
                Debug.LogError("invalid response code: " + components[1]);
            }
        }

        return ret;
    }

    /// <summary>
    /// Used to have some way to identify the requests (because they are asynchornous)
    /// </summary>
    /// <returns></returns>
    private int generateRequestID()
    {
        this.requestID++;
        return requestID;
    }


    private void setUserData(WWW www)
    {
        if (Constants.USE_FAKE_API)
        {
            DataManager.instance.NickName = "Nikky";
            DataManager.instance.Gold = 9999;
            DataManager.instance.Experience = 9999;
            DataManager.instance.Level = Constants.LEVELING_MAX_LEVEL;
            LoadingScreenManager.LoadScene(Constants.SCENE_NAME_SPLASH_SCENE);
            //SceneManager.LoadScene(Constants.SCENE_NAME_SPLASH_SCENE);
        }
        else
        {
            var parsed = JSON.Parse(www.text);

            Debug.Log(www.text);

            Debug.Log(parsed["nick"]);

            DataManager.instance.NickName = parsed["nick"];
            DataManager.instance.Gold = parsed["gold"].AsInt;
            DataManager.instance.Experience = parsed["experience"].AsInt;
            DataManager.instance.ExperienceToLevelUP = parsed["experience_to_level_up"].AsInt;
            DataManager.instance.Level = parsed["level"].AsInt;
            DataManager.instance.Diamonds = parsed["diamonds"].AsInt;
            Mob.MaxMobsToSpawn = DataManager.instance.Level * 5 > Constants.LEVELING_MAX_LEVEL ? Constants.LEVELING_MAX_LEVEL : DataManager.instance.Level * 5;

            if (Constants.DEVELOPMENT)
            {
                Debugger.instance.addDebugginInfo("Nick", DataManager.instance.NickName);
                Debugger.instance.addDebugginInfo("Money", DataManager.instance.Gold.ToString());
                Debugger.instance.addDebugginInfo("Experience", DataManager.instance.Experience.ToString());
                Debugger.instance.addDebugginInfo("Level", DataManager.instance.Level.ToString());
                Debugger.instance.addDebugginInfo("Diamonds", DataManager.instance.Diamonds.ToString());
            }
        }
        DataManager.instance.RequestGetInventory();
    }
}

