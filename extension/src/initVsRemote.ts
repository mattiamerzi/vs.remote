import { ProtoGrpcType } from './fs';
import { VsRemoteClient } from './vsremote/VsRemote';
import * as grpc from '@grpc/grpc-js';
import * as protoLoader from '@grpc/proto-loader';

export default function init_gRPC(remoteAddress: string): VsRemoteClient {
    const PROTO_PATH = __dirname + '/proto/fs.proto';
    console.log(`Vs.Remote gRPC proto file ${PROTO_PATH}`);

    const packageDefinition = protoLoader.loadSync(
        PROTO_PATH,
        {
            keepCase: false,
            longs: Number,
            enums: Number,
            defaults: true,
            oneofs: true
        });

    const protoDescriptor: ProtoGrpcType = (grpc.loadPackageDefinition(packageDefinition) as any) as ProtoGrpcType;
    const vsremote = protoDescriptor.vsremote;
    const options = { 'grpc.max_receive_message_length': 1024 * 1024 * 1024 * 1024}; // 1Tb, that means: if you need to set a limit, set it on the server side.
    const vsclient:VsRemoteClient = new vsremote.VsRemote(remoteAddress, grpc.credentials.createInsecure(), options);

    return vsclient;
}