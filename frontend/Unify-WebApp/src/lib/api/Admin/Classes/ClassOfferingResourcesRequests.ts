import { api } from "$lib/api/api";
import { ClassType, ResourceType, type ClassResource } from "$lib/types/resources";

export interface CreateClassOfferingResourceRequest{
    classOfferingId: string,
    title: string,
    description: string,
    attachments: File[] | null,
}

export interface UpdateClassOfferingResourceRequest{
    id: string,
    title: string,
    description: string,
    attachments: File[] | null,
}

export interface DeleteClassOfferingResourceRequest{
    id: string,
}


export const CreateClassOfferingResource = async (data: CreateClassOfferingResourceRequest, token: string) => {
    const formData = new FormData();
    formData.append('classOfferingId', data.classOfferingId);
    formData.append('title', data.title);
    formData.append('description', data.description);
    if(data.attachments)
        data.attachments.forEach(file => {
            formData.append('attachments', file);
    });

    return await api('POST', `/class-offering/${data.classOfferingId}/resources`, formData, token, true);
}

export const UpdateClassOfferingResource = async (data: UpdateClassOfferingResourceRequest, token: string) => {
    const formData = new FormData();
    formData.append('title', data.title);
    formData.append('description', data.description);
    if(data.attachments)
        data.attachments.forEach(file => {
            formData.append('attachments', file);
    });

    return await api('PUT', `/class-offering/resources/${data.id}`, formData, token, true);
}

export const DeleteClassOfferingResource = async (data: DeleteClassOfferingResourceRequest, token: string) => {
    return await api('DELETE', `/class-offering/resources/${data.id}`, null, token);
}

export const GetClassOfferingResource = async (resourceId: string, token: string) : Promise<ClassResource> => {
    let resource = await api<ClassResource>('GET', `/class-offering/resources/${resourceId}`, null, token);
    resource.resourceType = ResourceType.classOffering;
    return resource;
}

export const GetClassOfferingResources = async (classOfferingId: string, token: string) : Promise<ClassResource[]> => {
    let resources = await api<ClassResource[]>('GET', `/class-offering/${classOfferingId}/resources`, null, token);
    resources.forEach(resource => {
        resource.resourceType = ResourceType.classOffering;
    });

    return resources;
}