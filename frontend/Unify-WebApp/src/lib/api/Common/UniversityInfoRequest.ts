import { api } from "../api";

export interface UniversityInformation{
    name: string
    abbreviation: string
}

export const getUniversityInformation = async () => { 
    return await api<UniversityInformation>('GET', '/about') }