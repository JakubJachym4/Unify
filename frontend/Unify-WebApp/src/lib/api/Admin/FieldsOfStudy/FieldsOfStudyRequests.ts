import { api } from '$lib/api/api';
import type { FieldOfStudy } from '$lib/types/university';

export const getAllFieldsOfStudy = async (token: string) => {
    return await api<FieldOfStudy[]>('GET','/fieldofstudies', null, token
    )};

export const addFieldOfStudy = async (data: { name: string, description: string, facultyId: string }, token: string)=>{
    return await api<string>('POST', '/fieldofstudies', data, token);}

export const updateFieldOfStudy = async (id: string, data: { name: string, description: string }, token: string) => {
    return await api('PUT', `/fieldofstudies/${id}`, { id, ...data }, token);}

export const deleteFieldOfStudy = async (id: string, token: string) =>{
    return await api('DELETE', `/fieldofstudies/${id}`, null, token);}