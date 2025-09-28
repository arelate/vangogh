package cli

import (
	"errors"
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
)

type userAction int

const (
	userActionUnknown userAction = iota
	userActionCreate
	userActionDelete
	userActionList
)

func UsersHandler(u *url.URL) error {

	q := u.Query()

	var action userAction
	username := q.Get("username")
	password := q.Get("password")
	role := q.Get("role")

	switch q.Get("action") {
	case "create":
		action = userActionCreate
		if username == "" || password == "" || role == "" {
			return errors.New("username, password and role must be provided")
		}
	case "delete":
		action = userActionDelete
		if username == "" || password == "" {
			return errors.New("username, password must be provided")
		}
	case "list":
		action = userActionList
	default:
		return errors.New("unknown user action: " + q.Get("action"))
	}

	return Users(action, username, password, role)
}

func Users(action userAction, username, password, role string) error {

	var actionVerb string
	switch action {
	case userActionCreate:
		actionVerb = "creating"
	case userActionDelete:
		actionVerb = "deleting"
	case userActionList:
		actionVerb = "listing"
	default:
		return errors.New("unknown user action")
	}

	ua := nod.Begin("%s users...", actionVerb)
	defer ua.Done()

	authorDir, err := pathways.GetAbsRelDir(vangogh_integration.Author)
	if err != nil {
		return err
	}

	auth, err := author.NewAuthenticator(authorDir, perm.GetRolesPermissions())
	if err != nil {
		return err
	}

	switch action {
	case userActionCreate:
		if err = auth.CreateUser(username, password); err != nil {
			return err
		}

		if err = auth.GrantRole(username, password, role); err != nil {
			return err
		}
	case userActionDelete:
		if err = auth.CutUser(username, password); err != nil {
			return err
		}
	case userActionList:
		userRoles := auth.GetUserRoles()

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
