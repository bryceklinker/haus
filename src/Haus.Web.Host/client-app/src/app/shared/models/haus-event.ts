export interface HausEvent<T = any> {
  type: string;
  payload: T;
}
