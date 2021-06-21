import firebase from "../firebase";
import { showLoadingSpinner, hideLoadingSpinner } from "../states/loading";
import { getParameterByName } from "../shared/querystring-helper";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";

const validate = (email, password) => {
  const emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  if (!email) return { success: false, input: "email"};
  if (!password) return { success: false, input: "password"};
  if (typeof email != "string") return { success: false, input: "email" };
  if (typeof password != "string") return { success: false, input: "email"};
  if (!emailRegex.test(email.toLowerCase())) return { success: false, input: "email"};
  return { success: true, message: "" };
};

export const login = async (email, password) => {
    const validationResponse = validate(email, password);
    const returnUrl = getParameterByName("ReturnUrl");
    let error;
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

            if (authResp.fetchResponse != fetchResponses.SUCCESS) {
                throw ({ code: "server" });
            }
            if (returnUrl && returnUrl.startsWith("/")) {
                window.location.href = returnUrl;
            } else {
                window.location.href = "/account/";
            }

        } catch (e) {
            if (e.code == "server") {
                error = 'server';
            } else {
                error = 'login';
            }
        }
    } else {
        error = validationResponse.input;
    }

    if (error) {
        if (window.location.pathname.startsWith('/login')) {
            window.location.href = `${window.location.pathname}?email=${email}&er=${error}&ReturnUrl=${encodeURIComponent(returnUrl)}`;
        } else {
            window.location.href = `/login?email=${email}&er=${error}&ReturnUrl=${encodeURIComponent(returnUrl)}`;
        }
    }
};

export default { login };
