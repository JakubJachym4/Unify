import { api } from "$lib/api/api"
import type { Faculty } from "$lib/types/university";

export interface UpdateFacultyRequest {
    id: string;
    name: string;
}

export const AddFaculty = async (name: string , token: string) => {
    return await api<string>('POST', '/faculties',  {name: name}, token)
}

export const UpdateFaculty = async (data: UpdateFacultyRequest, token: string) => {
    return await api('PUT',`/faculties/${data.id}`, data, token)
}

export const DeleteFaculty = async (id: string, token: string) => {
    return await api('DELETE',`/faculties/${id}`, null, token)
}

export const GetAllFaculties = async (token: string) => {
    return await api<Faculty[]>('GET','/faculties', null, token)
}