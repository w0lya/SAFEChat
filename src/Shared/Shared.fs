namespace Shared
    open System

    #if FABLE_COMPILER
    open Thoth.Json
    #else
    open Thoth.Json.Net
    #endif


    // TODO: figure out shared modules later.
    // presumably: types, security, messaging
    // module Types =

    type Counter = int


    static member Encode (msg: Msg) : string =
        let data =
            match msg with
            | Increment ->
                Encode.object [ "msg", Encode.string "increment"]
            | Decrement ->
                Encode.object [ "msg", Encode.string "decrement"]
            | _ ->
                Encode.object []

        Encode.toString 4 data

    static member Decode (json: string) : Option<Msg> =
        let decodeMsg =
            (Decode.field "msg" Decode.string)
            |> Decode.map (fun str ->
                            match str with
                            | "increment" -> Increment
                            | "decrement" -> Decrement
                            | _ -> Decrement
                          )
        let result = Decode.fromString decodeMsg json

        match result with
        | Ok msg ->
            Some msg
        | Error err ->
            None

    // Authorization types
    type RoleType = Visitor | Moderator | Admin    

    type PermissionType = 
        PostMessage 
        | JoinChannel | CreateChannel 
        | KickUser | AddUser 

    type RoleDefinition = {
        roleType : RoleType;
        permissions : PermissionType list;
    }

    // Chat context types
    type ChannelType =  Public | OneToOne

    type User = { 
        id : Guid; 
        role : RoleDefinition; 
        name : string; 
        email: string; 
        avatar : string; 
    }

    type Message = { 
        id : Guid; 
        userId: Guid; 
        time : DateTime;
        channelId : Guid;
        body : string;
    }

    type Channel = {
        id : Guid;
        participants : User list;
        moderators : User list;
        admin : User;
        channelType : ChannelType;
        name : string;
    }    

    // Messaging types
    type EventType = 
    MessagePosted | MessageUpdated | MessageDeleted | 
    UserJoined | UserLeft | UserRoleAssigned

    type Event = 
        | MessagePosted of Message // add or update
        | MessageDeleted of { initiatorUserId : Guid; messageId : Guid; channelId : Guid; } // userId - who is requeting to deleted it
        | UserJoined of { "joined"; userId : Guid; channelId : Guid; }
        | UserLeft of { "left"; userId : Guid; channelId : Guid; }
        | RoleAssignmentUpdated of {initiatorUserId : Guid; updtedUser : User;} 
        


    