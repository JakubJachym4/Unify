export interface FieldOfStudy {
    id: string;
    name: string;
    description: string;
    facultyId: string;
    ClassOfferings: object[];
}

export interface Specialization {
    id: string;
    name: string; 
    description: string;
    fieldOfStudyId: string;
}

export interface Faculty {
    id: string;
    name: string;
}

export interface AcademicLocation {
    id: string;
    building?: string;
    street?: string;
    floor?: number;
    doorNumber?: string;
    facultyId?: string;
    online: boolean;
    meetingUrl?: string;
}