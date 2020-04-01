import * as firebase from "firebase/app";
import "firebase/auth";

class clientFirebase {
  static app;
  static auth;
  static init(firebaseConfig) {
    try {
      clientFirebase.app = firebase.initializeApp(firebaseConfig);
      clientFirebase.auth = firebase.auth();
    } catch (e) {
      console.error(`An error occurred initialising firebase app: ${e.message}`);
    }
  }
}

export default clientFirebase;
