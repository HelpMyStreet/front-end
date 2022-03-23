import firebase from "firebase/compat/app";
import "firebase/compat/auth";

class clientFirebase {
  static app;
  static auth;
  static init(firebaseConfig) {
    try {
      clientFirebase.app = firebase.initializeApp(firebaseConfig);
      clientFirebase.auth = firebase.auth();
      clientFirebase.auth.setPersistence(firebase.auth.Auth.Persistence.SESSION);
    } catch (e) {
      console.error(`An error occurred initialising firebase app: ${e.message}`);
    }
  }
}

export default clientFirebase;
