import { universityInformation } from "./stores/university";

export const Load = async () => {
    await universityInformation.load();
    return {}
  }