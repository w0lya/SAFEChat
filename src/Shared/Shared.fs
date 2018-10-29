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

    // Event of different types   
    type Event = 
        | MessagePosted of Message // add or update
        | MessageDeleted of { initiatorUserId : Guid; messageId : Guid; channelId : Guid; } // userId - who is requeting to deleted it
        | UserJoined of { "joined"; userId : Guid; channelId : Guid; }
        | UserLeft of { "left"; userId : Guid; channelId : Guid; }
        | RoleAssignmentUpdated of {initiatorUserId : Guid; updtedUser : User;} 

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

        


    