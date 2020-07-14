import {createSelector} from 'reselect';
import {removeActionPostFix} from '../state/actions';

const initialState = {};

const MODAL_ACTION_CONVENTION_REGEX = /(.*)\s(Success|Open|Cancelled)/;

export function modalReducer(state = initialState, action) {
    const values = extractModalValues(action.type);
    if (values) {
        return {
            ...state,
            [values.modalName]: values.modalState === 'Open'
        }
    }
    return state;
}

const selectModalState = state => state.modals;
export const selectIsModalOpen = createSelector(
    selectModalState,
    (_, actionCreator) => actionCreator.type,
    (modalState, type) => {
        const modalName = removeActionPostFix(type);
        return modalState[modalName];
    }
)

function extractModalValues(type) {
    const matches = MODAL_ACTION_CONVENTION_REGEX.exec(type);
    if (matches) {
        const [, modalName, modalState] = matches;
        return {modalName, modalState};
    }
    return null;
}