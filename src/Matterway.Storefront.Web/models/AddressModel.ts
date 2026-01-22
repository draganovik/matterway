export default class AddressModel {
  receiverName: string;
  residence: string;
  street: string;
  city: string;
  zipCode: string;
  country?: string;
  contactPhone?: string;
  note?: string;

  constructor(data?: Partial<AddressModel>) {
    this.receiverName = data?.receiverName ?? "";
    this.residence = data?.residence ?? "";
    this.street = data?.street ?? "";
    this.city = data?.city ?? "";
    this.zipCode = data?.zipCode ?? "";
    this.country = data?.country ?? "";
    this.contactPhone = data?.contactPhone ?? "";
    this.note = data?.note ?? "";
  }

  validate() {
    const requiredFields = [
      this.receiverName,
      this.residence,
      this.street,
      this.city,
      this.zipCode,
    ];
    return requiredFields.every((field) => field?.trim().length);
  }
}
