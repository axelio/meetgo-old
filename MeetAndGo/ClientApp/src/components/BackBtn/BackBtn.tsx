import { faChevronCircleLeft } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Link } from 'react-router-dom';

import './BackBtn.css';

const BackBtn: React.FC<{ link?: string }> = ({ link }) =>
    <Link className='back-btn' to={`/${link}`}><FontAwesomeIcon size={"3x"} icon={faChevronCircleLeft} /></Link>

export default BackBtn;