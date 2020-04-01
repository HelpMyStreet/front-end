import '../sass/main.scss';
import * as firebase from 'firebase/app';
import 'firebase/auth';

$(() => {
    var firebaseConfig = {
        apiKey: "AIzaSyBcXGTnRXhFGq3fb6-ulyo-7qL8P0RIbqA",
        authDomain: "factor50-test.firebaseapp.com",
        databaseURL: "https://factor50-test.firebaseio.com",
        projectId: "factor50-test",
        storageBucket: "factor50-test.appspot.com",
        messagingSenderId: "1075949051901",
        appId: "1:1075949051901:web:1be61ff6f6de11c1934394"
    };

    firebase.initializeApp(firebaseConfig);

    $('#user_creation_form').length && $('#user_creation_form').on('submit', function (evt) {
        evt.preventDefault();

        const {
            email,
            password,
            confirm_password
        } = $(this).find('.input').get().reduce((acc, cur) => {
            const inp = $(cur).find('input');
            const { name, value, type } = inp[0];

            const errDisplay = $(cur).find('.error');
            errDisplay && errDisplay.text('').hide();

            acc[name] = {
                val: type === 'checkbox' ? value === 'on' : value,
                el: $(cur)
            };

            return acc;
        }, {});

        const pwValid = validatePassword(password.val, confirm_password.val);
        if (pwValid !== true) setError(confirm_password.el, pwValid);

        var btn = $(this).find('button[type=submit]');

        buttonLoad(btn);

        var auth = firebase.auth();

        auth.createUserWithEmailAndPassword(email.val, password.val)
            .then(user => {
                fetch('/register/1', {
                    method: 'post',
                    body: {
                        email: email.val,
                        
                    }
                })
            })
            .catch(err => {
                console.log(err);
                switch (err.code) {
                    case 'auth/email-already-in-use':
                        setError(email.el, err.message);
                        break;
                    case 'auth/weak-password':
                        setError(password.el, err.message);
                        break;
                }
            })
            .finally(() => buttonUnload(btn));
        
        return false;
    });
});

function setError(inputElement, message) {
    inputElement.find('.error').text(message).show()
}

function buttonLoad(btn) {
    btn.width(btn.width());
    btn.height(btn.height());
    btn.find('.text').hide();
    btn.find('.loader').show();
}

function buttonUnload(btn) {
    btn.width(null);
    btn.height(null);
    btn.find('.text').show();
    btn.find('.loader').hide();
}

function validatePassword(pass, check) {
    return pass === check || 'Please ensure the passwords match';
}