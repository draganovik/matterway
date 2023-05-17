export default class LoginModel {
  public email: string;
  public password: string;

  constructor() {
    this.email = "";
    this.password = "";
  }

  public validate(): boolean {
    const regex = new RegExp(
      "^[a-zA-Z0-9._:$!%-]+@[a-zA-Z0-9.-]+.[a-zA-Z]$"
    )
    
    if (this.email.length == 0) {
      return false;
    }
    if (!regex.test(this.email)) {
      return false;
    }
    if (this.password.length == 0) {
      return false;
    }
    return true;
  }
}
