import { api } from "$lib/api/api";
import { ClassType, ResourceType, type ClassResource } from "$lib/types/resources";

export interface CreateCourseResourceRequest{
    courseId: string,
    title: string,
    description: string,
    attachments: File[] | null,
}

export interface UpdateCourseResourceRequest{
    id: string,
    title: string,
    description: string,
    attachments: File[] | null,
}

export interface DeleteCourseResourceRequest{
    id: string,
}


export const CreateCourseResource = async (data: CreateCourseResourceRequest, token: string) => {
    const formData = new FormData();
    formData.append('courseId', data.courseId);
    formData.append('title', data.title);
    formData.append('description', data.description);
    if(data.attachments)
        data.attachments.forEach(file => {
            formData.append('attachments', file);
    });

    return await api('POST', `/courses/${data.courseId}/resources`, formData, token, true);
}

export const UpdateCourseResource = async (data: UpdateCourseResourceRequest, token: string) => {
    const formData = new FormData();
    formData.append('title', data.title);
    formData.append('description', data.description);
    if(data.attachments)
        data.attachments.forEach(file => {
            formData.append('attachments', file);
    });

    return await api('PUT', `/courses/resources/${data.id}`, formData, token, true);
}

export const DeleteCourseResource = async (data: DeleteCourseResourceRequest, token: string) => {
    return await api('DELETE', `/courses/resources/${data.id}`, null, token);
}

export const GetCourseResource = async (resourceId: string, token: string) : Promise<ClassResource> => {
    let resource = await api<ClassResource>('GET', `/courses/resources/${resourceId}`, null, token);
    resource.resourceType = ResourceType.course;
    return resource;
}

export const GetCourseResources = async (courseId: string, token: string) : Promise<ClassResource[]> => {
    let resources = await api<ClassResource[]>('GET', `/courses/${courseId}/resources`, null, token);
    resources.forEach(resource => {
        resource.resourceType = ResourceType.course;
    });

    return resources;
}