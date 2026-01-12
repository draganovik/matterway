export default class JwtModel {
  sub!: string;
  role!: string;
  nbf!: number;
  exp!: number;
  iat!: number;
}
