import type { Attachment } from "./resources";

export interface ClassResource{
    id: string,
    title: string,
    description: string,
    attachments: Attachment[],
    resourceType: ResourceType,
}

export enum ResourceType{
    course,
    classOffering
}

export enum ClassType{
    class,
    lecture
}

export interface ClassSession{
    id: string,
    parentId: string,
    title: string,
    scheduledDate: string,
    duration: number,
    lecturerId: string,
    locationId: string,
    classType: ClassType,
}
export type Attachment = {
    fileName: string;
    contentType: string;
    data: string;
};

export const convertFilesToAttachments = (files: File[]): Attachment[] => {
    return files.map(file => ({
        fileName: file.name,
        contentType: file.type,
        data: ''
    }));
};

export const convertAttachmentsToFiles = (attachments: Attachment[]): File[] => {
    return attachments.map(attachment => new File([attachment.data], attachment.fileName, { type: attachment.contentType }));
};

