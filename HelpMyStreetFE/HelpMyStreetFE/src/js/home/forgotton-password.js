import { validateFormData } from "../shared/validator";
import { showLoadingSpinner, hideLoadingSpinner } from "../states/loading"

export function intialiseForgottonForm(firebase, account) {    
    $('#forgotton-form').on("submit", function (evt) {
  
        $(".error").hide();
        const valid = validateFormData($(this), {
            email: (v) => v !== "" || "Please enter a valid email address"
        });   
        evt.preventDefault();        
        
        let email = $(this).find("input[name='email']");
        if (valid) {            
            showLoadingDiv();
            firebase.auth.sendPasswordResetEmail(email.val()).then(function () {
                showEmailSentScreen();
            }).catch(function (error) {         
                if (error !== undefined && error.code !== undefined) {          
                    console.log(error.code);
                    switch (error.code) {
                        case 'auth/invalid-email':
                            hideLoadingDiv();
                            $(".error").show();
                            $(".error").text("Please enter a valid email address");                            
                            break;
                        case 'auth/user-not-found': // we should not give indication if account is not found.                            
                            showEmailSentScreen();
                            break;
                        case 'auth/too-many-requests':
                            hideLoadingDiv();
                            $(".error").show();
                            $(".error").text("You have exceed your reset password limit");
                            break;

                    }
                }                
            });
        }
    })


    $('#reset-password-form').on("submit", function (evt) {
        const valid = validateFormData($(this), {
            password: (v) =>
                RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{10,})").test(v) ||
                "Please use a strong password",
        });

        if (!valid) return false;
        evt.preventDefault();                   
        if (typeof configuration == 'undefined' && configuration.actionCode == 'undefined') return false;
        if (!valid) return false;
        showLoadingDiv();
        var actionCode = configuration.actionCode;
        firebase.auth.verifyPasswordResetCode(actionCode).then(function (email) {
            var accountEmail = email;                                    
            var newPassword = $("input[name='password']").val();            
            firebase.auth.confirmPasswordReset(actionCode, newPassword).then(function (resp) {
                showPassResetScreen();
                account.login.login(accountEmail, newPassword).then(function (response) {
                });
                hideLoadingDiv()
            }).catch(function (error) {
                // catch if firebase password is too weak, but we have our own validation so we shouldnt be hitting here
                hideLoadingDiv();
            });
        }).catch(function (error) {
                showExpiredScreen();
        });
    });
}

function showEmailSentScreen() {
    $('#forgotton_success').show();
    $('#forgotton-form').hide();
}

function showExpiredScreen() {
    $('#expired_link').show();
    $('#reset-password-form').hide(); 
}

function showPassResetScreen() {
    $('#logging-in').show();
    $('#reset-password-form').hide(); 
}

function showLoadingDiv(){
    $('#submitbtn').hide();
    showLoadingSpinner(".forgotton__page__button");
}

function hideLoadingDiv() {
    $('#submitbtn').show();
    hideLoadingSpinner(".forgotton__page__button");
}
