namespace Shared
    open System

    #if FABLE_COMPILER
    open Thoth.Json
    #else
    open Thoth.Json.Net
    #endif

    // TODO: figure out shared modules later.
    // presumably: types, auth, messaging
    // module Types =

    type Counter = int

    
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

    // TODO: revisit
    type ChannelActionType = 
        UserHasJoined | UserHasLeft 

    type ChannelAction = {
        actionType : ChannelActionType;
        userId : Guid;
        channelId : Guid;
    }

    type MessageAction = {
        userId : Guid; // author or moderator / admin
        message : Message; 
    }

    // Event of different types   
    type Event = 
        | MessagePosted of MessageAction // add or update
        | MessageDeleted of MessageAction
        | UserJoined of ChannelAction
        | UserLeft of ChannelAction
        | RoleAssignmentUpdated of Guid * User // guid - initiation user id 

        // TODO: a more elegant way of doing this?
        static member Encode (event: Event) : string =
            let data =
                match event with
                | MessagePosted -> 
                    Encode.object ["event", Encode.string "MessagePosted" ]
                | MessageDeleted ->
                    Encode.object ["event", Encode.string "MessageDeleted" ]
                | UserJoined ->
                    Encode.object ["event", Encode.string "UserJoined" ]
                | UserLeft ->
                    Encode.object ["event", Encode.string "UserLeft" ]
                | RoleAssignmentUpdated ->
                    Encode.object ["event", Encode.string "RoleAssignmentUpdated" ]
                | _ -> Encode.object []

            Encode.toString 4 data

        static member Decode (json: string) : Option<Event> =
            let decodeEvent =
                (Decode.field "event" Decode.string)
                |> Decode.map (fun str ->
                                match str with
                                | "MessagePosted" -> MessagePosted
                                | "MessageDeleted" -> MessageDeleted
                                | "UserJoined" -> UserJoined
                                | "UserLeft" -> UserLeft
                                | "RoleAssignmentUpdated" -> RoleAssignmentUpdated
                                | _ -> Decrement
                            )
            let result = Decode.fromString decodeEvent json

            match result with
            | Ok event ->
                Some event
            | Error err ->
                None

        


    