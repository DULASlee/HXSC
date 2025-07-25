import CryptoJS from "crypto-js";

const SECRET_KEY = "SmartConstructionSaaS2024";

export function encrypt(text: string): string {
  return CryptoJS.AES.encrypt(text, SECRET_KEY).toString();
}

export function decrypt(ciphertext: string): string {
  const bytes = CryptoJS.AES.decrypt(ciphertext, SECRET_KEY);
  return bytes.toString(CryptoJS.enc.Utf8);
}

export function md5(text: string): string {
  return CryptoJS.MD5(text).toString();
}
