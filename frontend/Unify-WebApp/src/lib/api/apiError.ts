export type ApiRequestError = Error & {
    response: Response;
    code: string;
    details: string;
};
