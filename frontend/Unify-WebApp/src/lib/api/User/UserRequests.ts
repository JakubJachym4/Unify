import { api } from '../api'
import type { ApiRequestError } from '../apiError';

interface RegisterUserRequest {
    email: string;
    firstName: string;
    lastName: string;
    password: string;
}

export interface UserResponse{
    id: string
    email: string,
    firsName: string,
    lastName: string,
}

export const registerUser = async (data: RegisterUserRequest) => {
    return api('POST', '/users/register', data);
};

interface LogInUserRequest {
    email: string;
    password: string;
}

export const logInUser = async (data: LogInUserRequest) => {
    try {
        return await api('POST', '/users/login', data);
    } catch (error) {
        const apiError = error as ApiRequestError;
        if (apiError.response.status === 401) {
            throw new Error('Wrong email or password');
        }
        throw error;
    }
};

export const logOutUser = async (token: string) => {
    return api('POST', '/users/logout', null, token);
};

export const getUserData = async (token: string) => {
    return api('GET', '/users/me', null, token)
}
