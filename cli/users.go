package cli

import (
	"errors"
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
	"github.com/boggydigital/nod"
)

type userAction int

const (
	userActionUnknown userAction = iota
	userActionCreate
	userActionChangePassword
	userActionDelete
	userActionList
)

func UsersHandler(u *url.URL) error {

	q := u.Query()

	username := q.Get("username")
	password := q.Get("password")
	newPassword := q.Get("new-password")
	role := q.Get("role")

	action := userActionUnknown
	if q.Has("create") {
		action = userActionCreate
	} else if q.Has("delete") {
		action = userActionDelete
	} else if q.Has("change-password") {
		action = userActionChangePassword
	} else if q.Has("list") {
		action = userActionList
	}

	switch action {
	case userActionCreate:
		if username == "" || password == "" || role == "" {
			return errors.New("username, password and role must be provided")
		}
	case userActionDelete:
		if username == "" || password == "" {
			return errors.New("username, password must be provided")
		}
	case userActionChangePassword:
		if username == "" || password == "" || newPassword == "" {
			return errors.New("username, password and new-password must be provided")
		}
	case userActionList:
		// do nothing
	default:
		return errors.New("unknown user action: " + q.Get("action"))
	}

	return Users(action, username, password, newPassword, role)
}

func Users(action userAction, username, password, newPassword, role string) error {

	var actionVerb string
	switch action {
	case userActionCreate:
		actionVerb = "creating"
	case userActionDelete:
		actionVerb = "deleting"
	case userActionChangePassword:
		actionVerb = "changing password for"
	case userActionList:
		actionVerb = "listing"
	default:
		return errors.New("unknown user action")
	}

	ua := nod.Begin("%s users...", actionVerb)
	defer ua.Done()

	authorDir := vangogh_integration.Pwd.AbsRelDirPath(vangogh_integration.Author, vangogh_integration.Metadata)

	auth, err := author.NewAuthenticator(authorDir, perm.GetRolesPermissions())
	if err != nil {
		return err
	}

	switch action {
	case userActionCreate:
		if err = auth.CreateUser(username, password); err != nil {
			return err
		}
		if err = auth.SetRole(username, password, role); err != nil {
			return err
		}
	case userActionDelete:
		if err = auth.CutUser(username, password); err != nil {
			return err
		}
	case userActionChangePassword:
		if err = auth.UpdatePassword(username, password, newPassword); err != nil {
			return err
		}
	case userActionList:
		var userRoles map[string][]string
		userRoles, err = auth.GetUserRoles()
		if err != nil {
			return err
		}

		if len(userRoles) > 0 {
			ua.EndWithSummary("users and roles:", userRoles)
		} else {
			ua.EndWithResult("no users found")
		}
	default:
		return errors.New("unknown user action")
	}

	return nil
}
