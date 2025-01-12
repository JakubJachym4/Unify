import type { AcademicLocation } from "$lib/types/university";
import { api } from "../api";

export interface AddLocationRequest {
    building: string;
    street: string;
    floor: number;
    doorNumber: string;
    facultyId?: string;
}

export interface AddOnlineLocationRequest {
    meetingUrl: string;
}

export interface UpdateLocationRequest {
    id: string;
    building: string;
    street: string;
    floor: number;
    doorNumber: string;
    facultyId?: string;
}

export interface UpdateOnlineLocationRequest {
    id: string;
    meetingUrl: string;
}

export const AddLocation = async (data: {
    building: string;
    street: string;
    floor: number;
    doorNumber: string;
    facultyId?: string;
}, token: string) => {
    return await api<string>('POST', '/locations', data, token)
}

export const AddOnlineLocation = async (meetingUrl: string, token: string) => {
    return await api<string>('POST', '/locations/online', { meetingUrl }, token)
}

export const UpdateLocation = async (data: UpdateLocationRequest, token: string) => {
    return await api('PUT', `/locations/${data.id}`, data, token)
}

export const UpdateOnlineLocation = async (data: UpdateOnlineLocationRequest, token: string) => {
    return await api('PUT', `/locations/online/${data.id}`, data, token)
}

export const DeleteLocation = async (id: string, token: string) => {
    return await api('DELETE', `/locations/${id}`, null, token)
}

export const GetAllLocations = async (token: string) => {
    return await api<AcademicLocation[]>('GET', '/locations', null, token)
}