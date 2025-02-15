export interface ClassResource{
    id: string,
    title: string,
    description: string,
    attachments: Attachment[],
    resourceType: ResourceType,
}

export interface HomeworkAssignment{
    id: string,
    classOfferingId: string,
    title: string,
    description: string,
    dueDate: string,
    attachments: Attachment[],
}

export interface HomeworkSubmission{
    id: string,
    assignmentId: string,
    studentId: string,
    submissionDate: string,
    attachments: Attachment[],
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

export const convertFilesToAttachments = (files: File[] | null): Attachment[] => {
    if (!files) {
        return [];
    }
    return files.map(file => ({
        fileName: file.name,
        contentType: file.type,
        data: ''
    }));
};

export const convertAttachmentsToFiles = (attachments: Attachment[] | null): File[] => {
    if (!attachments) {
        return [];
    }
    return attachments.map(attachment => new File([attachment.data], attachment.fileName, { type: attachment.contentType }));
};

