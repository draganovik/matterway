export default class AddressModel {
  receiverName!: string;
  residence!: string;
  street!: string;
  city!: string;
  zipCode!: string;
  note?: string;

  validate() {
    if (this.receiverName.length == 0) {
      return false;
    }
    if (this.residence.length == 0) {
      return false;
    }
    if (this.street.length == 0) {
      return false;
    }
    if (this.city.length == 0) {
      return false;
    }
    if (this.zipCode.length == 0) {
      return false;
    }
    return true;
  }
}
