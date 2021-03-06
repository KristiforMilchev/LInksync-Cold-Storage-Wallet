/// <reference types="node" />
export declare function bufferToArray(buf: Buffer): Uint8Array;
export declare function bufferToHex(buf: Buffer, prefixed?: boolean): string;
export declare function bufferToUtf8(buf: Buffer): string;
export declare function bufferToNumber(buf: Buffer): number;
export declare function bufferToBinary(buf: Buffer): string;
export declare function arrayToBuffer(arr: Uint8Array): Buffer;
export declare function arrayToHex(arr: Uint8Array, prefixed?: boolean): string;
export declare function arrayToUtf8(arr: Uint8Array): string;
export declare function arrayToNumber(arr: Uint8Array): number;
export declare function arrayToBinary(arr: Uint8Array): string;
export declare function hexToBuffer(hex: string): Buffer;
export declare function hexToArray(hex: string): Uint8Array;
export declare function hexToUtf8(hex: string): string;
export declare function hexToNumber(hex: string): number;
export declare function hexToBinary(hex: string): string;
export declare function utf8ToBuffer(utf8: string): Buffer;
export declare function utf8ToArray(utf8: string): Uint8Array;
export declare function utf8ToHex(utf8: string, prefixed?: boolean): string;
export declare function utf8ToNumber(utf8: string): number;
export declare function utf8ToBinary(utf8: string): string;
export declare function numberToBuffer(num: number): Buffer;
export declare function numberToArray(num: number): Uint8Array;
export declare function numberToHex(num: number, prefixed?: boolean): string;
export declare function numberToUtf8(num: number): string;
export declare function numberToBinary(num: number): string;
export declare function binaryToBuffer(bin: string): Buffer;
export declare function binaryToArray(bin: string): Uint8Array;
export declare function binaryToHex(bin: string | string, prefixed?: boolean): string;
export declare function binaryToUtf8(bin: string): string;
export declare function binaryToNumber(bin: string): number;
export declare function isBinaryString(str: any): boolean;
export declare function isHexString(str: any, length?: number): boolean;
export declare function isBuffer(val: any): boolean;
export declare function isTypedArray(val: any): boolean;
export declare function isArrayBuffer(val: any): boolean;
export declare function getType(val: any): "string" | "number" | "bigint" | "boolean" | "symbol" | "undefined" | "object" | "function" | "buffer" | "array" | "typed-array" | "array-buffer";
export declare function getEncoding(str: string): "hex" | "utf8" | "binary";
export declare function concatBuffers(...args: Buffer[]): Buffer;
export declare function concatArrays(...args: Uint8Array[]): Uint8Array;
export declare function trimLeft(data: Buffer, length: number): Buffer;
export declare function trimRight(data: Buffer, length: number): Buffer;
export declare function calcByteLength(length: number, byteSize?: number): number;
export declare function splitBytes(str: string, byteSize?: number): string[];
export declare function swapBytes(str: string): string;
export declare function swapHex(str: string): string;
export declare function sanitizeBytes(str: string, byteSize?: number, padding?: string): string;
export declare function padLeft(str: string, length: number, padding?: string): string;
export declare function padRight(str: string, length: number, padding?: string): string;
export declare function removeHexPrefix(hex: string): string;
export declare function addHexPrefix(hex: string): string;
export declare function sanitizeHex(hex: string): string;
export declare function removeHexLeadingZeros(hex: string): string;
//# sourceMappingURL=index.d.ts.map