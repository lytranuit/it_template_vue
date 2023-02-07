import axios from "axios";
export function useAxios() {
  const axiosinstance = axios.create({
    baseURL: "https://localhost:44332/",
  });
  return { axiosinstance };
}
