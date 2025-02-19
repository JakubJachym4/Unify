import { api } from "$lib/api/api";

export interface CreateMarkRequest{
    gradeId: string,
    title: string,
    score: number,
    maxScore: number,
}

export const CreateMark = async (data: CreateMarkRequest, token: string) => {
    return await api('POST', `/grades/${data.gradeId}/marks`, data, token);
}

export const AwardGrade = async (gradeId: string, award: boolean,  token: string) => {
    return await api('PUT', `/grades/${gradeId}/award=${award}`, null , token);
}