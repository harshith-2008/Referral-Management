export interface LoginDTO {
  email: string;
  password: string;
}

export interface LoginResponseDTO {
  userId: number;
  email: string;
  roleId: number;
  token: string;
}
