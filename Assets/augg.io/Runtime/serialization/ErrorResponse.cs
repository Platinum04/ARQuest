using System;
using UnityEngine;


namespace Auggio.Utils.Serialization
{
    [Serializable]
    public class ErrorResponse
    {
        [SerializeField] private string message;
        [SerializeField] private ErrorPopupCode errorCode;

        public string Message
        {
            get => message;
            set => message = value;
        }

        public ErrorPopupCode ErrorCode
        {
            get => errorCode;
            set => errorCode = value;
        }
        
        public ErrorResponse(string message, ErrorPopupCode errorCode)
        {
            this.message = message;
            this.errorCode = errorCode;
        }
        

        public static ErrorResponse Get(string responseBody)
        {
            try
            {
                ErrorResponse response = JsonUtility.FromJson<ErrorResponse>(responseBody);
                if (response == null) {
                    return new ErrorResponse(responseBody, ErrorPopupCode.Unspecified);
                }
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError("Could not parse error response to model: " + responseBody);
                return new ErrorResponse(responseBody, ErrorPopupCode.Unspecified);;
            }
        }
    }
    
    public enum ErrorPopupCode
    {
        Unspecified = -1,
        AG000 = 0,
        AG001 = 1, 
        AG002 = 2, 
        AG003 = 3, 
        AG004 = 4, 
        AG005 = 5, 
        AG006 = 6,
        AG007 = 7,
        AG008 = 8,
        AG009 = 9,
        AG010 = 10,
        AG011 = 11,
        AG012 = 12,
        
        
        GO000 = 100,
        GO001 = 101,
        
        A001 = 1001,
    }
}

