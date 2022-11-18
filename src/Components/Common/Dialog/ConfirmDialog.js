import React, { useEffect, useState } from "react";
import Button from "react-bootstrap/Button";
import Modal from "react-bootstrap/Modal";

const ConfirmDialog = (props) => {
  const { setShowConfirm, title, content, funcAgree } = props;

  useEffect(() => {
    setShow(true);
  }, [setShowConfirm, title, content]);

  const [show, setShow] = useState(false);

  const handleClose = () => {
    setShow(false);
    setShowConfirm(false);
  };

  const handleOnClickAgree = () => {
    funcAgree();
  };

  return (
    <>
      <Modal
        show={show}
        onHide={handleClose}
        backdrop="static"
        keyboard={false}
      >
        <Modal.Header>
          <Modal.Title>{title}</Modal.Title>
        </Modal.Header>
        <Modal.Body>{content}</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>
            Đóng
          </Button>
          <Button variant="primary" onClick={() => handleOnClickAgree()}>
            Đồng Ý
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};

export default ConfirmDialog;
