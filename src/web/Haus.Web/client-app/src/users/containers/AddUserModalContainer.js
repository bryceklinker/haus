import {connect} from 'react-redux';
import {AddUserModalView} from '../views/AddUserModalView';
import {selectIsModalOpen} from '../../shared/modals/modals.reducer';
import {addUser} from '../state/users.actions';

function mapStateToProps(state) {
    return {
        isOpen: selectIsModalOpen(state, addUser.open)
    }
}

function mapDispatchToProps(dispatch) {
    return {
        onConfirm: (user) => dispatch(addUser.confirm(user)),
        onCancel: () => dispatch(addUser.cancel())
    }
}
export const AddUserModalContainer = connect(mapStateToProps, mapDispatchToProps)(AddUserModalView);