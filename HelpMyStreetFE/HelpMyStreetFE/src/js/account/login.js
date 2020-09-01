import firebase from "../firebase";
import { showLoadingSpinner, hideLoadingSpinner } from "../states/loading";
import { getParameterByName } from "../shared/querystring-helper";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";
const handleErrorResponse = response => {    
    if (response) {    
    switch (response.code) {
        case "auth/wrong-password":
            return { success: false, message: "Incorrect username or password provided, please try again" };
            break;
        case "auth/user-not-found":
            return { success: false, message: "Incorrect username or password provided, please try again" };
            break;
      default:
        return { success: false, message: "An unexpected error occurred" };
    }
  }
};

const validate = (email, password) => {
  const emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  if (!email) return { success: false, message: "Please provide an email address" };
  if (!password) return { success: false, message: "Please provide a password" };
  if (typeof email != "string") return { success: false, message: "Please provide a valid email address" };
  if (typeof password != "string") return { success: false, message: "Please provide a valid email address" };
  if (!emailRegex.test(email.toLowerCase())) return { success: false, message: "Please provide a valid email address" };
  //TODO: Add check for password e.g. length, special characters etc
  return { success: true, message: "" };
};

export const login = async (email, password) => {
  showLoadingSpinner('.header-login__form');
  
  const validationResponse = validate(email, password);
  if (validationResponse.success) {
    try {
      const credentials = await firebase.auth.signInWithEmailAndPassword(email, password);
        
        const token = await credentials.user.getIdToken();

      const authResp = await hmsFetch('/api/auth', {
        method: 'post',
        headers: {
          authorization: `Bearer ${token}`,
          'content-type': 'application/json'
        },
        body: JSON.stringify({ token })
      });
        console.log("Fetch Rsp:" + authResp.fetchResponse);
        switch (authResp.fetchResponse) {
            case fetchResponses.SERVER_ERROR:
                throw ({ code: "auth/internal-server-error" });
                break;
            case fetchResponses.UNAUTHORISED:
                throw ({ code: "auth/server-not-authorised" });
                break;
            case fetchResponses.TIMEOUT:
                throw ({ code: "auth/server-timeout" });
                break;

        }
      
        var returnUrl = getParameterByName("ReturnUrl");        
        if (returnUrl) {
            window.location.href = returnUrl;
        } else {
            window.location.href = "/account";
        }
        return { success: true };
    } catch (e) {              
      hideLoadingSpinner('.header-login__form');
      return handleErrorResponse(e);
    }
  } else {
    hideLoadingSpinner('.header-login__form');  
    return validationResponse;
  }
};

export default { login };
