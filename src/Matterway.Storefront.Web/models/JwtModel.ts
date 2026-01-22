export default class JwtModel {
  sub!: string;
  role!: string;
  perm?: string[] | string;
  nbf!: number;
  exp!: number;
  iat!: number;
}
