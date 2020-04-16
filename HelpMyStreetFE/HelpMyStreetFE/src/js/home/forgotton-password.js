import { validateFormData } from "../shared/validator";
import { showLoadingSpinner, hideLoadingSpinner } from "../states/loading"
import { buttonLoad, buttonUnload } from "../shared/btn";
export function intialiseForgottonForm(firebase, account) {    
    $('#forgotton-form').on("submit", function (evt) {
  
        $(".error").hide();
        const valid = validateFormData($(this), {
            email: (v) => v !== "" || "Please enter a valid email address"
        });   
        evt.preventDefault();        
        
        let email = $(this).find("input[name='email']");
        if (valid) {        
            
            buttonLoad($('#submitbtn'))
            firebase.auth.sendPasswordResetEmail(email.val()).then(function () {
                showEmailSentScreen();
            }).catch(function (error) {         
                if (error !== undefined && error.code !== undefined) {                     
                    switch (error.code) {
                        case 'auth/invalid-email':
                            buttonUnload($('#submitbtn'))
                            $(".error").show();
                            $(".error").text("Please enter a valid email address");                            
                            break;
                        case 'auth/user-not-found': // we should not give indication if account is not found.                            
                            showEmailSentScreen();
                            break;
                        case 'auth/too-many-requests':
                            buttonUnload($('#submitbtn'))
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
        buttonLoad($('#submitbtn'))
        var actionCode = configuration.actionCode;
        firebase.auth.verifyPasswordResetCode(actionCode).then(function (email) {
            var accountEmail = email;                                    
            var newPassword = $("input[name='password']").val();            
            firebase.auth.confirmPasswordReset(actionCode, newPassword).then(function (resp) {
                showPassResetScreen();
                account.login.login(accountEmail, newPassword).then(function (response) {
                });
                buttonUnload($('#submitbtn'))
            }).catch(function (error) {
                // catch if firebase password is too weak, but we have our own validation so we shouldnt be hitting here
                buttonUnload($('#submitbtn'))
            });
        }).catch(function (error) {
            showExpiredScreen();
            buttonUnload($('#submitbtn'))
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
    $().hide();

    buttonLoad('#submitbtn');
    showLoadingSpinner(".forgotton__page__button");
}

function hideLoadingDiv() {
        
}
