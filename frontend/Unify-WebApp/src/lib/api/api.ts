import type { ApiRequestError } from './apiError';

export const api: {
    (method: string, endpoint: string, data?: any, token?: string | null, isMultipart?: boolean) : Promise<any>;
    <T>(method: string, endpoint: string, data?: any, token?: string | null, isMultipart?: boolean): Promise<T>;
} = async <T>(
    method: string,
    endpoint: string,
    data: any = null,
    token: string | null = null,
    isMultipart: boolean = false
): Promise<T> => {
    const headers: Record<string, string> = {};

    if (!isMultipart) {
        headers['Content-Type'] = 'application/json';
    }

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    endpoint = "https://localhost:5001/api" + endpoint;
    const res = await fetch(endpoint, {
        method,
        headers,
        body: isMultipart ? data : (data ? JSON.stringify(data) : null),
    });

    if (!res.ok) {
        const errorData = await res.json();
        const error: ApiRequestError = new Error('API call failed') as ApiRequestError;
        error.response = res;
        error.code = errorData.code;
        error.details = errorData.details;

        if(!error.details){
            try{
                error.details = errorData.errors[0].errorMessage;
            }
            catch(Error){}
        }

        throw error;
    }

    try{
        const responseData: T = await res.json();
        return responseData;
    }catch(Error){}
    
    return null as T;
}