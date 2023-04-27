import React, { useRef } from "react";
import { Button } from "react-bootstrap";
import ReactToPrint from "react-to-print";
import ReportPage, { ComponentToPrint } from "../../Report/ReportPage";
import { useReactToPrint } from "react-to-print";

export default function PrintComponent() {
  const componentRef = useRef();
  const handlePrint = useReactToPrint({
    content: () => componentRef.current,
  });

  return (
    <>
      <div>
        <div style={{ display: "none" }}></div>
        <button onClick={handlePrint}>Print this out!</button>
      </div>
    </>
  );
}
