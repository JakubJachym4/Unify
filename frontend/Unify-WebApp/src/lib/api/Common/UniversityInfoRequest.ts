import { api } from "../api";

export interface UniversityInformation{
    name: string
    abbreviation: string
}

export interface StudentInformationResponse{
    id: string
    firstName: string
    lastName: string
    email: string
    studentGroup: string,
    fieldOfStudy: string,
    specialization: string,
    semester: number,
    term: string,
    studyYear: string,
    faculty: string,
}

export const getUniversityInformation = async () => { 
    return await api<UniversityInformation>('GET', '/about') 
}

export const getStudentInformation = async (token: string) => {
    return await api<StudentInformationResponse>('GET', '/about/me', null, token)
}