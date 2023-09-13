export interface Message {
    id: number;
    senderId: number;
    senderUserNAme: string;
    senderPhotoUrl: string;
    recicipientId: number;
    recipientUsername: string;
    recipientPhotoUrl: string;
    content: string;
    dateRead?: Date;
    messageSent: Date;
  }
  