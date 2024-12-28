import { api } from '../api'

interface RegisterUserRequest {
    email: string;
    firstName: string;
    lastName: string;
    password: string;
}

export interface UserResponse{
    id: string
    email: string,
    firstName: string,
    lastName: string,
    roles: string[],
    profileImage: string
}

export const registerUser = async (data: RegisterUserRequest) => {
    return api('POST', '/users/register', data);
};

interface LogInUserRequest {
    email: string;
    password: string;
}

export const logInUser = async (data: LogInUserRequest) => {
    return await api('POST', '/users/login', data);
};

export const logOutUser = async (token: string) => {
    return await api('POST', '/users/logout', null, token);
};

export const getUserData = async (token: string) => {
    return await api<UserResponse>('GET', '/users/me', null, token);
}

export const getAllUsers = async (token: string) => {
    return await api<UserResponse[]>('GET', '/users/users', null, token);
}